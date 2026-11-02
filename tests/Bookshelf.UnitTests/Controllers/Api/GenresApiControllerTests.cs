using Bookshelf.Application.Common;
using Bookshelf.Application.Genres;
using Bookshelf.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers.Api;

public class GenresApiControllerTests
{
    private static readonly GenreDetailsDto Genre = new(3, "Dystopia", []);

    private readonly Mock<IGenreService> service = new();
    private readonly GenresApiController controller;

    public GenresApiControllerTests()
    {
        controller = new GenresApiController(service.Object);
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetAll_ReturnsGenres()
    {
        service.Setup(s => s.GetAllAsync(Ct)).ReturnsAsync([new GenreListDto(3, "Dystopia", 2)]);

        var result = await controller.GetAll(Ct);

        result.Should().ContainSingle().Which.Name.Should().Be("Dystopia");
    }

    [Fact]
    public async Task Get_Existing_ReturnsGenre()
    {
        service.Setup(s => s.GetByIdAsync(3, Ct)).ReturnsAsync(Result<GenreDetailsDto>.Success(Genre));

        var result = await controller.Get(3, Ct);

        result.Value.Should().Be(Genre);
    }

    [Fact]
    public async Task Get_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<GenreDetailsDto>.NotFound("Жанр не знайдено."));

        var result = await controller.Get(42, Ct);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_Success_ReturnsCreated()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateGenreDto>(), Ct)).ReturnsAsync(Result<int>.Success(3));
        service.Setup(s => s.GetByIdAsync(3, Ct)).ReturnsAsync(Result<GenreDetailsDto>.Success(Genre));

        var result = await controller.Create(new CreateGenreDto { Name = "Dystopia" }, Ct);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.RouteValues!["id"].Should().Be(3);
        created.Value.Should().Be(Genre);
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsBadRequest()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateGenreDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("Name", "Такий жанр уже існує.")));

        var result = await controller.Create(new CreateGenreDto { Name = "Dystopia" }, Ct);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>().Which.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Update_Success_ReturnsNoContent()
    {
        service.Setup(s => s.UpdateAsync(It.Is<UpdateGenreDto>(d => d.Id == 3), Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Update(3, new UpdateGenreDto { Name = "Антиутопія" }, Ct);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ValidationError_ReturnsBadRequest()
    {
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateGenreDto>(), Ct))
            .ReturnsAsync(Result.Invalid(ControllerTestExtensions.Errors("Name", "Вкажіть назву жанру.")));

        var result = await controller.Update(3, new UpdateGenreDto(), Ct);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.DeleteAsync(42, Ct)).ReturnsAsync(Result.NotFound("Жанр не знайдено."));

        var result = await controller.Delete(42, Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_GenreWithBooks_ReturnsConflict()
    {
        service.Setup(s => s.DeleteAsync(3, Ct)).ReturnsAsync(Result.Conflict("Не можна видалити жанр, у якому є книги."));

        var result = await controller.Delete(3, Ct);

        result.Should().BeOfType<ConflictObjectResult>();
    }
}
