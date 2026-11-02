using Bookshelf.Application.Authors;
using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Application.Genres;
using Bookshelf.Web.Controllers;
using Bookshelf.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> books = new();
    private readonly BooksController controller;

    public BooksControllerTests()
    {
        var authors = new Mock<IAuthorService>();
        authors.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new AuthorListDto(1, "George Orwell", "UK", 1903, 1)]);
        var genres = new Mock<IGenreService>();
        genres.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new GenreListDto(1, "Dystopia", 1)]);

        controller = new BooksController(books.Object, authors.Object, genres.Object).WithTempData();
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Index_PassesFilterToService()
    {
        books.Setup(s => s.GetAllAsync(It.IsAny<BookFilter>(), Ct)).ReturnsAsync([]);

        var result = await controller.Index(genreId: 1, authorId: 2, search: "farm", Ct);

        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<BookIndexViewModel>();
        books.Verify(s => s.GetAllAsync(new BookFilter { GenreId = 1, AuthorId = 2, Search = "farm" }, Ct));
    }

    [Fact]
    public async Task Details_Missing_ReturnsNotFound()
    {
        books.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<BookDetailsDto>.NotFound("Книгу не знайдено."));

        var result = await controller.Details(42, Ct);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_WithCover_PassesFileToService()
    {
        CreateBookDto? sent = null;
        books.Setup(s => s.CreateAsync(It.IsAny<CreateBookDto>(), Ct))
            .Callback<CreateBookDto, CancellationToken>((dto, _) => sent = dto)
            .ReturnsAsync(Result<int>.Success(9));
        var form = BookForm();
        form.Cover = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "Cover", "cover.png");

        var result = await controller.Create(form, Ct);

        result.Should().BeOfType<RedirectToActionResult>().Which.RouteValues!["id"].Should().Be(9);
        sent!.Cover!.FileName.Should().Be("cover.png");
    }

    [Fact]
    public async Task Create_ValidationError_ReturnsViewWithDropdowns()
    {
        books.Setup(s => s.CreateAsync(It.IsAny<CreateBookDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("Isbn", "Книга з таким ISBN уже є.")));

        var result = await controller.Create(BookForm(), Ct);

        var model = result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<BookFormViewModel>().Subject;
        model.Authors.Should().NotBeEmpty();
        controller.ModelState["Isbn"]!.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Delete_Success_RedirectsToIndex()
    {
        books.Setup(s => s.DeleteAsync(1, Ct)).ReturnsAsync(Result.Success());

        var result = await controller.DeleteConfirmed(1, Ct);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(nameof(BooksController.Index));
    }

    private static BookFormViewModel BookForm() => new()
    {
        Title = "Animal Farm",
        PublishedYear = 1945,
        Isbn = "9780306406157",
        Pages = 112,
        AuthorId = 1,
        GenreId = 1,
    };
}
