using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Application.Reviews;
using Bookshelf.Web.Controllers;
using Bookshelf.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers;

public class ReviewsControllerTests
{
    private readonly Mock<IReviewService> service = new();
    private readonly Mock<IBookService> books = new();
    private readonly ReviewsController controller;

    public ReviewsControllerTests()
    {
        controller = new ReviewsController(service.Object, books.Object).WithTempData();
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Create_Success_RedirectsToBook()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateReviewDto>(), Ct)).ReturnsAsync(Result<int>.Success(3));

        var result = await controller.Create(new ReviewFormViewModel { BookId = 1, ReaderName = "Olena", Rating = 5 }, Ct);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ControllerName.Should().Be("Books");
        redirect.RouteValues!["id"].Should().Be(1);
    }

    [Fact]
    public async Task Create_ValidationError_ReturnsViewWithSelectedBook()
    {
        books.Setup(s => s.GetByIdAsync(1, Ct)).ReturnsAsync(Result<BookDetailsDto>.Success(
            new BookDetailsDto(1, "Animal Farm", null, 1945, "9780306406157", 112, null, 1, "George Orwell", 1, "Dystopia", null, [])));
        service.Setup(s => s.CreateAsync(It.IsAny<CreateReviewDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("Rating", "Оцінка має бути від 1 до 5.")));

        var result = await controller.Create(new ReviewFormViewModel { BookId = 1, ReaderName = "Olena" }, Ct);

        var model = result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<ReviewFormViewModel>().Subject;
        model.BookTitle.Should().Be("Animal Farm");
        model.CanChooseBook.Should().BeTrue();
        controller.ModelState["Rating"]!.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Delete_Success_RedirectsToBook()
    {
        service.Setup(s => s.DeleteAsync(3, Ct)).ReturnsAsync(Result.Success());

        var result = await controller.DeleteConfirmed(3, bookId: 1, Ct);

        result.Should().BeOfType<RedirectToActionResult>().Which.RouteValues!["id"].Should().Be(1);
    }
}
