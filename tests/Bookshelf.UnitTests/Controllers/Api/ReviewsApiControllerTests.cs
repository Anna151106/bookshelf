using Bookshelf.Application.Common;
using Bookshelf.Application.Reviews;
using Bookshelf.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers.Api;

public class ReviewsApiControllerTests
{
    private static readonly ReviewDto Review = new(5, 1, "1984", "Оксана", 5, "Чудово", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private readonly Mock<IReviewService> service = new();
    private readonly ReviewsApiController controller;

    public ReviewsApiControllerTests()
    {
        controller = new ReviewsApiController(service.Object).WithRequest("/api/reviews");
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetAll_ReturnsPage()
    {
        service.Setup(s => s.GetPageAsync(0, 10, Ct)).ReturnsAsync(new PagedList<ReviewDto>([Review], 1));

        var result = await controller.GetAll(new PageQuery(), Ct);

        result.Items.Should().Equal(Review);
    }

    [Fact]
    public async Task Get_Existing_ReturnsReview()
    {
        service.Setup(s => s.GetByIdAsync(5, Ct)).ReturnsAsync(Result<ReviewDto>.Success(Review));

        var result = await controller.Get(5, Ct);

        result.Value.Should().Be(Review);
    }

    [Fact]
    public async Task Get_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<ReviewDto>.NotFound("Відгук не знайдено."));

        var result = await controller.Get(42, Ct);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_Success_ReturnsCreated()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateReviewDto>(), Ct)).ReturnsAsync(Result<int>.Success(5));
        service.Setup(s => s.GetByIdAsync(5, Ct)).ReturnsAsync(Result<ReviewDto>.Success(Review));

        var result = await controller.Create(new CreateReviewDto { BookId = 1, ReaderName = "Оксана", Rating = 5 }, Ct);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.RouteValues!["id"].Should().Be(5);
        created.Value.Should().Be(Review);
    }

    [Fact]
    public async Task Create_ValidationError_ReturnsBadRequest()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateReviewDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("Rating", "Оцінка має бути від 1 до 5.")));

        var result = await controller.Create(new CreateReviewDto { BookId = 1, ReaderName = "Оксана", Rating = 9 }, Ct);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>().Which.Errors.Should().ContainKey("Rating");
    }

    [Fact]
    public async Task Update_Success_ReturnsNoContent()
    {
        service.Setup(s => s.UpdateAsync(It.Is<UpdateReviewDto>(d => d.Id == 5), Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Update(5, new UpdateReviewDto { ReaderName = "Оксана", Rating = 4 }, Ct);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateReviewDto>(), Ct)).ReturnsAsync(Result.NotFound("Відгук не знайдено."));

        var result = await controller.Update(42, new UpdateReviewDto { ReaderName = "Оксана", Rating = 4 }, Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_Success_ReturnsNoContent()
    {
        service.Setup(s => s.DeleteAsync(5, Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Delete(5, Ct);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.DeleteAsync(42, Ct)).ReturnsAsync(Result.NotFound("Відгук не знайдено."));

        var result = await controller.Delete(42, Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
