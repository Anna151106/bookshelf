using Bookshelf.Application.Authors;
using Bookshelf.Application.Common;
using Bookshelf.Web.Controllers;
using Bookshelf.Web.Infrastructure;
using Bookshelf.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorService> service = new();
    private readonly AuthorsController controller;

    public AuthorsControllerTests()
    {
        controller = new AuthorsController(service.Object).WithTempData();
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Details_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<AuthorDetailsDto>.NotFound("Автора не знайдено."));

        var result = await controller.Details(42, Ct);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_Success_RedirectsToDetails()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateAuthorDto>(), Ct)).ReturnsAsync(Result<int>.Success(7));

        var result = await controller.Create(new AuthorFormViewModel { FullName = "Ray Bradbury" }, Ct);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(AuthorsController.Details));
        redirect.RouteValues!["id"].Should().Be(7);
    }

    [Fact]
    public async Task Create_ValidationError_ReturnsViewWithErrors()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateAuthorDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("FullName", "Вкажіть ім'я автора.")));

        var result = await controller.Create(new AuthorFormViewModel(), Ct);

        result.Should().BeOfType<ViewResult>();
        controller.ModelState["FullName"]!.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Edit_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateAuthorDto>(), Ct)).ReturnsAsync(Result.NotFound("Автора не знайдено."));

        var result = await controller.Edit(42, new AuthorFormViewModel { FullName = "X" }, Ct);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_AuthorWithBooks_ShowsError()
    {
        service.Setup(s => s.DeleteAsync(1, Ct)).ReturnsAsync(Result.Conflict("Не можна видалити автора, у якого є книги."));

        var result = await controller.DeleteConfirmed(1, Ct);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(nameof(AuthorsController.Details));
        controller.TempData[ControllerResultExtensions.ErrorKey].Should().Be("Не можна видалити автора, у якого є книги.");
    }

    [Fact]
    public async Task Delete_Success_RedirectsToIndex()
    {
        service.Setup(s => s.DeleteAsync(1, Ct)).ReturnsAsync(Result.Success());

        var result = await controller.DeleteConfirmed(1, Ct);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(nameof(AuthorsController.Index));
    }
}
