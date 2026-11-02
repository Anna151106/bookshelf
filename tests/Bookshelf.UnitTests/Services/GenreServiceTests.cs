using Bookshelf.Application.Common;
using Bookshelf.Application.Genres;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class GenreServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly GenreService service;

    public GenreServiceTests()
    {
        var db = database.Context;
        service = new GenreService(db, new CreateGenreDtoValidator(db), new UpdateGenreDtoValidator(db));
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task Create_DuplicateName_ReturnsValidationError()
    {
        database.AddGenre("Dystopia");

        var result = await service.CreateAsync(new CreateGenreDto { Name = "dystopia" }, Ct);

        result.Status.Should().Be(ResultStatus.ValidationError);
    }

    [Fact]
    public async Task Update_KeepingSameName_Succeeds()
    {
        var genre = database.AddGenre("Dystopia");

        var result = await service.UpdateAsync(new UpdateGenreDto { Id = genre.Id, Name = "Dystopia" }, Ct);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_GenreWithBooks_ReturnsConflict()
    {
        var genre = database.AddGenre();
        database.AddBook(database.AddAuthor(), genre);

        var result = await service.DeleteAsync(genre.Id, Ct);

        result.Status.Should().Be(ResultStatus.Conflict);
        (await database.CreateContext().Genres.FindAsync([genre.Id], Ct)).Should().NotBeNull();
    }
}
