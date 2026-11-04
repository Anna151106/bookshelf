using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Authors;
using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Application.Genres;
using Bookshelf.Infrastructure.Persistence;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class LookupSearchTests : IDisposable
{
    private readonly TestDatabase database = new();

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task Authors_MatchesPartOfWord_IgnoringCase()
    {
        var orwell = database.AddAuthor("George Orwell");
        database.AddAuthor("Ray Bradbury");
        var service = new AuthorService(database.Context, new CreateAuthorDtoValidator(TimeProvider.System), new UpdateAuthorDtoValidator(TimeProvider.System));

        var result = await service.SearchAsync("ORWE", Ct);

        result.Should().Equal(new LookupItemDto(orwell.Id, "George Orwell"));
    }

    [Fact]
    public async Task Genres_MatchesInsideWord_IgnoringCase()
    {
        var dystopia = database.AddGenre("Dystopia");
        database.AddGenre("Drama");
        var db = database.Context;
        var service = new GenreService(db, new CreateGenreDtoValidator(db), new UpdateGenreDtoValidator(db));

        var result = await service.SearchAsync("sTOp", Ct);

        result.Should().Equal(new LookupItemDto(dystopia.Id, "Dystopia"));
    }

    [Fact]
    public async Task Books_ReturnsAtMostTwentyMatches()
    {
        var author = database.AddAuthor();
        var genre = database.AddGenre();
        for (var i = 1; i <= 25; i++)
        {
            database.AddBook(author, genre, $"Book {i:D2}", $"97800000{i:D5}");
        }

        database.AddBook(author, genre, "Other", "9780000099999");
        var service = CreateBookService(database.Context);

        var result = await service.SearchAsync("book", Ct);

        result.Should().HaveCount(Lookup.MaxResults)
            .And.OnlyContain(x => x.Text.StartsWith("Book "));
        result[0].Text.Should().Be("Book 01");
    }

    [Fact]
    public async Task Books_BlankQuery_ReturnsNothing()
    {
        database.AddBook(database.AddAuthor(), database.AddGenre());
        var service = CreateBookService(database.Context);

        var result = await service.SearchAsync("  ", Ct);

        result.Should().BeEmpty();
    }

    private static BookService CreateBookService(AppDbContext db) =>
        new(db, Mock.Of<IFileStorage>(), new CreateBookDtoValidator(db, TimeProvider.System), new UpdateBookDtoValidator(db, TimeProvider.System));
}
