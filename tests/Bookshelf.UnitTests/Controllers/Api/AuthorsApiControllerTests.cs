using Bookshelf.Application.Authors;
using Bookshelf.Application.Common;
using Bookshelf.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers.Api;

public class AuthorsApiControllerTests
{
    private static readonly AuthorDetailsDto Author = new(7, "Ray Bradbury", null, 1920, "USA", []);

    private readonly Mock<IAuthorService> service = new();
    private readonly AuthorsApiController controller;

    public AuthorsApiControllerTests()
    {
        controller = new AuthorsApiController(service.Object).WithRequest("/api/authors");
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetAll_ReturnsRequestedPage()
    {
        service.Setup(s => s.GetPageAsync(20, 10, Ct))
            .ReturnsAsync(new PagedList<AuthorListDto>([new AuthorListDto(1, "George Orwell", "UK", 1903, 2)], 35));

        var result = await controller.GetAll(new PageQuery { Skip = 20, Limit = 10 }, Ct);

        result.Items.Should().ContainSingle().Which.FullName.Should().Be("George Orwell");
        result.Total.Should().Be(35);
        result.NextLink.Should().Be("http://localhost/api/authors?skip=30&limit=10");
    }

    [Fact]
    public async Task Get_Existing_ReturnsAuthor()
    {
        service.Setup(s => s.GetByIdAsync(7, Ct)).ReturnsAsync(Result<AuthorDetailsDto>.Success(Author));

        var result = await controller.Get(7, Ct);

        result.Value.Should().Be(Author);
    }

    [Fact]
    public async Task Get_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<AuthorDetailsDto>.NotFound("Автора не знайдено."));

        var result = await controller.Get(42, Ct);

        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>().Which.Detail.Should().Be("Автора не знайдено.");
    }

    [Fact]
    public async Task Create_Success_ReturnsCreated()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateAuthorDto>(), Ct)).ReturnsAsync(Result<int>.Success(7));
        service.Setup(s => s.GetByIdAsync(7, Ct)).ReturnsAsync(Result<AuthorDetailsDto>.Success(Author));

        var result = await controller.Create(new CreateAuthorDto { FullName = "Ray Bradbury" }, Ct);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(AuthorsApiController.Get));
        created.RouteValues!["id"].Should().Be(7);
        created.Value.Should().Be(Author);
    }

    [Fact]
    public async Task Create_ValidationError_ReturnsBadRequestWithFieldErrors()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateAuthorDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("FullName", "Вкажіть ім'я автора.")));

        var result = await controller.Create(new CreateAuthorDto(), Ct);

        var problem = result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>().Subject;
        problem.Errors["FullName"].Should().Equal("Вкажіть ім'я автора.");
    }

    [Fact]
    public async Task Update_Success_UsesRouteIdAndReturnsNoContent()
    {
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateAuthorDto>(), Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Update(7, new UpdateAuthorDto { Id = 99, FullName = "Ray Bradbury" }, Ct);

        result.Should().BeOfType<NoContentResult>();
        service.Verify(s => s.UpdateAsync(It.Is<UpdateAuthorDto>(d => d.Id == 7), Ct));
    }

    [Fact]
    public async Task Update_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateAuthorDto>(), Ct)).ReturnsAsync(Result.NotFound("Автора не знайдено."));

        var result = await controller.Update(42, new UpdateAuthorDto { FullName = "X" }, Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_Success_ReturnsNoContent()
    {
        service.Setup(s => s.DeleteAsync(7, Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Delete(7, Ct);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_AuthorWithBooks_ReturnsConflict()
    {
        service.Setup(s => s.DeleteAsync(1, Ct)).ReturnsAsync(Result.Conflict("Не можна видалити автора, у якого є книги."));

        var result = await controller.Delete(1, Ct);

        result.Should().BeOfType<ConflictObjectResult>()
            .Which.Value.Should().BeOfType<ProblemDetails>().Which.Status.Should().Be(409);
    }
}
