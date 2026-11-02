using Bookshelf.Application.Authors;
using Bookshelf.Application.Common;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class AuthorServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly AuthorService service;

    public AuthorServiceTests()
    {
        var time = FixedTimeProvider.Default;
        service = new AuthorService(database.Context, new CreateAuthorDtoValidator(time), new UpdateAuthorDtoValidator(time));
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task Create_SavesAuthor()
    {
        var result = await service.CreateAsync(new CreateAuthorDto { FullName = "  Ray Bradbury ", BirthYear = 1920 }, Ct);

        result.IsSuccess.Should().BeTrue();
        var author = await database.CreateContext().Authors.FindAsync([result.Value], Ct);
        author!.FullName.Should().Be("Ray Bradbury");
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsValidationError()
    {
        var result = await service.CreateAsync(new CreateAuthorDto { FullName = " " }, Ct);

        result.Status.Should().Be(ResultStatus.ValidationError);
        result.ValidationErrors.Should().ContainKey(nameof(CreateAuthorDto.FullName));
    }

    [Fact]
    public async Task GetById_Missing_ReturnsNotFound()
    {
        var result = await service.GetByIdAsync(42, Ct);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Delete_AuthorWithBooks_ReturnsConflict()
    {
        var author = database.AddAuthor();
        database.AddBook(author, database.AddGenre());

        var result = await service.DeleteAsync(author.Id, Ct);

        result.Status.Should().Be(ResultStatus.Conflict);
        (await database.CreateContext().Authors.FindAsync([author.Id], Ct)).Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_AuthorWithoutBooks_RemovesAuthor()
    {
        var author = database.AddAuthor();

        var result = await service.DeleteAsync(author.Id, Ct);

        result.IsSuccess.Should().BeTrue();
        (await database.CreateContext().Authors.FindAsync([author.Id], Ct)).Should().BeNull();
    }
}
