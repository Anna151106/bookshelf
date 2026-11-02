using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Domain.Entities;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class BookServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly Mock<IFileStorage> fileStorage = new();
    private readonly BookService service;
    private readonly Author author;
    private readonly Genre genre;

    public BookServiceTests()
    {
        var db = database.Context;
        var time = FixedTimeProvider.Default;
        service = new BookService(db, fileStorage.Object, new CreateBookDtoValidator(db, time), new UpdateBookDtoValidator(db, time));

        author = database.AddAuthor();
        genre = database.AddGenre();
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task GetAll_FilterByGenre()
    {
        var sciFi = database.AddGenre("Science fiction");
        database.AddBook(author, genre, "Animal Farm", "9780000000019");
        database.AddBook(author, sciFi, "The Martian Chronicles", "9780000000026");

        var books = await service.GetAllAsync(new BookFilter { GenreId = sciFi.Id }, Ct);

        books.Should().ContainSingle().Which.Title.Should().Be("The Martian Chronicles");
    }

    [Fact]
    public async Task GetAll_FilterByAuthor()
    {
        var bradbury = database.AddAuthor("Ray Bradbury");
        database.AddBook(author, genre, "Animal Farm", "9780000000019");
        database.AddBook(bradbury, genre, "Fahrenheit 451", "9780000000026");

        var books = await service.GetAllAsync(new BookFilter { AuthorId = bradbury.Id }, Ct);

        books.Should().ContainSingle().Which.Title.Should().Be("Fahrenheit 451");
    }

    [Fact]
    public async Task GetAll_SearchByTitle_IgnoresCase()
    {
        database.AddBook(author, genre, "Animal Farm", "9780000000019");
        database.AddBook(author, genre, "Nineteen Eighty-Four", "9780000000026");

        var books = await service.GetAllAsync(new BookFilter { Search = "FARM" }, Ct);

        books.Should().ContainSingle().Which.Title.Should().Be("Animal Farm");
    }

    [Fact]
    public async Task GetById_CalculatesAverageRating()
    {
        var book = database.AddBook(author, genre);
        database.AddReview(book, rating: 5);
        database.AddReview(book, rating: 4);

        var result = await service.GetByIdAsync(book.Id, Ct);

        result.Value!.AverageRating.Should().Be(4.5);
    }

    [Fact]
    public async Task Create_WithCover_SavesFile()
    {
        fileStorage
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), "cover.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploads/covers/abc.jpg");

        var result = await service.CreateAsync(NewBook() with { Cover = new FileUpload(new MemoryStream([1]), "cover.jpg") }, Ct);

        result.IsSuccess.Should().BeTrue();
        (await database.CreateContext().Books.FindAsync([result.Value], Ct))!.CoverPath.Should().Be("uploads/covers/abc.jpg");
    }

    [Fact]
    public async Task Create_BadCover_ReturnsValidationErrorAndSavesNothing()
    {
        fileStorage
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FileStorageException("Обкладинка має бути у форматі JPG, PNG або WEBP."));

        var result = await service.CreateAsync(NewBook() with { Cover = new FileUpload(new MemoryStream(), "cover.gif") }, Ct);

        result.ValidationErrors.Should().ContainKey(nameof(CreateBookDto.Cover));
        database.CreateContext().Books.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_DuplicateIsbn_ReturnsValidationError()
    {
        database.AddBook(author, genre, isbn: "9780306406157");

        var result = await service.CreateAsync(NewBook() with { Isbn = "978-0-306-40615-7" }, Ct);

        result.ValidationErrors.Should().ContainKey(nameof(CreateBookDto.Isbn));
    }

    [Fact]
    public async Task Update_KeepingOwnIsbn_Succeeds()
    {
        var book = database.AddBook(author, genre, isbn: "9780306406157");

        var result = await service.UpdateAsync(UpdatedBook(book.Id), Ct);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithNewCover_DeletesOldFile()
    {
        var book = database.AddBook(author, genre, isbn: "9780306406157", coverPath: "uploads/covers/old.jpg");
        fileStorage
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), "new.png", It.IsAny<CancellationToken>()))
            .ReturnsAsync("uploads/covers/new.png");

        var result = await service.UpdateAsync(UpdatedBook(book.Id) with { Cover = new FileUpload(new MemoryStream([1]), "new.png") }, Ct);

        result.IsSuccess.Should().BeTrue();
        fileStorage.Verify(s => s.DeleteAsync("uploads/covers/old.jpg", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_RemovesReviewsAndCover()
    {
        var book = database.AddBook(author, genre, coverPath: "uploads/covers/abc.jpg");
        database.AddReview(book);

        var result = await service.DeleteAsync(book.Id, Ct);

        result.IsSuccess.Should().BeTrue();
        (await database.CreateContext().Reviews.AnyAsync(Ct)).Should().BeFalse();
        fileStorage.Verify(s => s.DeleteAsync("uploads/covers/abc.jpg", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Missing_ReturnsNotFound()
    {
        var result = await service.DeleteAsync(42, Ct);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private CreateBookDto NewBook() => new()
    {
        Title = "Animal Farm",
        PublishedYear = 1945,
        Isbn = "9780306406157",
        Pages = 112,
        AuthorId = author.Id,
        GenreId = genre.Id,
    };

    private UpdateBookDto UpdatedBook(int id) => new()
    {
        Id = id,
        Title = "Animal Farm",
        PublishedYear = 1945,
        Isbn = "9780306406157",
        Pages = 112,
        AuthorId = author.Id,
        GenreId = genre.Id,
    };
}
