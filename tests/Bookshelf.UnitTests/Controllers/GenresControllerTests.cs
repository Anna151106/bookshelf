using Bookshelf.Application.Common;
using Bookshelf.Application.Genres;
using Bookshelf.Web.Controllers;
using Bookshelf.Web.Infrastructure;
using Bookshelf.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers;

public class GenresControllerTests
{
    private readonly Mock<IGenreService> service = new();
    private readonly GenresController controller;

    public GenresControllerTests()
    {
        controller = new GenresController(service.Object).WithTempData();
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Create_DuplicateName_ReturnsViewWithError()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateGenreDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("Name", "Такий жанр уже є.")));

        var result = await controller.Create(new GenreFormViewModel { Name = "Drama" }, Ct);

        result.Should().BeOfType<ViewResult>();
        controller.ModelState["Name"]!.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Delete_GenreWithBooks_ShowsError()
    {
        service.Setup(s => s.DeleteAsync(1, Ct)).ReturnsAsync(Result.Conflict("Не можна видалити жанр, у якому є книги."));

        var result = await controller.DeleteConfirmed(1, Ct);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(nameof(GenresController.Details));
        controller.TempData[ControllerResultExtensions.ErrorKey].Should().NotBeNull();
    }
}
