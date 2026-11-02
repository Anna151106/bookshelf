using Bookshelf.Application.Reviews;
using Bookshelf.Domain.Entities;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class ReviewServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly ReviewService service;
    private readonly Book book;

    public ReviewServiceTests()
    {
        var db = database.Context;
        service = new ReviewService(db, FixedTimeProvider.Default, new CreateReviewDtoValidator(db), new UpdateReviewDtoValidator());

        book = database.AddBook(database.AddAuthor(), database.AddGenre());
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task Create_SetsCreatedAt()
    {
        var result = await service.CreateAsync(new CreateReviewDto { BookId = book.Id, ReaderName = "Olena", Rating = 5 }, Ct);

        var review = await database.CreateContext().Reviews.FindAsync([result.Value], Ct);
        review!.CreatedAt.Should().Be(FixedTimeProvider.Default.GetUtcNow().UtcDateTime);
    }

    [Fact]
    public async Task Create_MissingBook_ReturnsValidationError()
    {
        var result = await service.CreateAsync(new CreateReviewDto { BookId = 999, ReaderName = "Olena", Rating = 5 }, Ct);

        result.ValidationErrors.Should().ContainKey(nameof(CreateReviewDto.BookId));
    }

    [Fact]
    public async Task Delete_RemovesReview()
    {
        var review = database.AddReview(book);

        var result = await service.DeleteAsync(review.Id, Ct);

        result.IsSuccess.Should().BeTrue();
        (await database.CreateContext().Reviews.FindAsync([review.Id], Ct)).Should().BeNull();
    }
}
