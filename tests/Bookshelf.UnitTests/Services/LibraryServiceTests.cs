using Bookshelf.Application.Libraries;
using Bookshelf.Domain.Entities;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class LibraryServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly LibraryService service;
    private readonly Book farm;
    private readonly Book nineteen;
    private readonly Book hamlet;

    public LibraryServiceTests()
    {
        service = new LibraryService(database.Context);

        var orwell = database.AddAuthor("George Orwell");
        var shakespeare = database.AddAuthor("William Shakespeare");
        var dystopia = database.AddGenre("Dystopia");
        var drama = database.AddGenre("Drama");
        farm = database.AddBook(orwell, dystopia, "Animal Farm", "9780000000019");
        nineteen = database.AddBook(orwell, dystopia, "1984", "9780000000026");
        hamlet = database.AddBook(shakespeare, drama, "Hamlet", "9780000000033");

        database.AddLibrary("A", "Подільський", farm, hamlet);
        database.AddLibrary("B", "Печерський", nineteen, farm);
        database.AddLibrary("C", "Подільський", hamlet);
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task GetForMap_ByBook_ReturnsOnlyLibrariesWithThatBook()
    {
        var points = await service.GetForMapAsync(new LibraryMapFilter { BookId = farm.Id }, Ct);

        points.Select(p => p.Name).Should().Equal("A", "B");
        points.Should().AllSatisfy(p => p.Books.Should().Equal(new LibraryBookDto(farm.Id, "Animal Farm")));
    }

    [Fact]
    public async Task GetForMap_ByAuthor_CountsMatchingBooks()
    {
        var points = await service.GetForMapAsync(new LibraryMapFilter { AuthorId = farm.AuthorId }, Ct);

        points.Select(p => (p.Name, p.BookCount)).Should().Equal(("A", 1), ("B", 2));
        points[1].Books.Should().Equal(new LibraryBookDto(nineteen.Id, "1984"), new LibraryBookDto(farm.Id, "Animal Farm"));
    }

    [Fact]
    public async Task GetForMap_ByGenreAndDistrict_CombinesFilters()
    {
        var points = await service.GetForMapAsync(new LibraryMapFilter { GenreId = hamlet.GenreId, District = "Подільський" }, Ct);

        points.Select(p => (p.Name, p.BookCount)).Should().Equal(("A", 1), ("C", 1));
    }

    [Fact]
    public async Task GetForMap_ByDistrictOnly_ReturnsAllBooksOfLibraries()
    {
        var points = await service.GetForMapAsync(new LibraryMapFilter { District = "Печерський" }, Ct);

        points.Should().ContainSingle()
            .Which.BookCount.Should().Be(2);
    }
}
