using Bookshelf.Application.Statistics;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class StatisticsServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly StatisticsService service;

    public StatisticsServiceTests()
    {
        service = new StatisticsService(database.Context);
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task Get_CountsBooksByGenre_SkippingEmptyGenres()
    {
        var author = database.AddAuthor();
        var drama = database.AddGenre("Drama");
        var dystopia = database.AddGenre("Dystopia");
        database.AddGenre("Poetry");
        database.AddBook(author, dystopia, "1984", "9780000000026");
        database.AddBook(author, dystopia, "Animal Farm", "9780000000019");
        database.AddBook(author, drama, "Hamlet", "9780000000033");

        var stats = await service.GetAsync(Ct);

        stats.BooksByGenre.Should().Equal(
            new GenreBookCountDto("Drama", 1),
            new GenreBookCountDto("Dystopia", 2));
    }

    [Fact]
    public async Task Get_AveragesRatingsAcrossAllBooksOfAuthor()
    {
        var genre = database.AddGenre();
        var orwell = database.AddAuthor("George Orwell");
        var huxley = database.AddAuthor("Aldous Huxley");
        database.AddAuthor("No Reviews");
        var farm = database.AddBook(orwell, genre, "Animal Farm", "9780000000019");
        var nineteen = database.AddBook(orwell, genre, "1984", "9780000000026");
        var brave = database.AddBook(huxley, genre, "Brave New World", "9780000000033");
        database.AddReview(farm, 5);
        database.AddReview(farm, 4);
        database.AddReview(nineteen, 2);
        database.AddReview(brave, 3);

        var stats = await service.GetAsync(Ct);

        stats.AverageRatingByAuthor.Should().Equal(
            new AuthorRatingDto("Aldous Huxley", 3),
            new AuthorRatingDto("George Orwell", 3.67));
    }

    [Fact]
    public async Task Get_GroupsBooksByDecade()
    {
        var author = database.AddAuthor();
        var genre = database.AddGenre();
        database.AddBook(author, genre, "A", "9780000000019", publishedYear: 1949);
        database.AddBook(author, genre, "B", "9780000000026", publishedYear: 1940);
        database.AddBook(author, genre, "C", "9780000000033", publishedYear: 2003);

        var stats = await service.GetAsync(Ct);

        stats.BooksByDecade.Should().Equal(
            new DecadeBookCountDto(1940, 2),
            new DecadeBookCountDto(2000, 1));
    }
}
