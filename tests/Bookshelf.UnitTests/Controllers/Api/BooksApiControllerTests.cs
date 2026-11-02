using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers.Api;

public class BooksApiControllerTests
{
    private static readonly BookDetailsDto Book = new(
        1, "1984", "Антиутопія", 1949, "9780000000101", 328, null, 4, "George Orwell", 3, "Dystopia", null, []);

    private static readonly BookRequest Request = new()
    {
        Title = "1984",
        PublishedYear = 1949,
        Isbn = "9780000000101",
        Pages = 328,
        AuthorId = 4,
        GenreId = 3,
    };

    private readonly Mock<IBookService> service = new();
    private readonly BooksApiController controller;

    public BooksApiControllerTests()
    {
        controller = new BooksApiController(service.Object);
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetAll_ReturnsBooks()
    {
        service.Setup(s => s.GetAllAsync(null, Ct)).ReturnsAsync([new BookListDto(1, "1984", 1949, null, 4, "George Orwell", 3, "Dystopia")]);

        var result = await controller.GetAll(Ct);

        result.Should().ContainSingle().Which.Title.Should().Be("1984");
    }

    [Fact]
    public async Task Get_Existing_ReturnsBook()
    {
        service.Setup(s => s.GetByIdAsync(1, Ct)).ReturnsAsync(Result<BookDetailsDto>.Success(Book));

        var result = await controller.Get(1, Ct);

        result.Value.Should().Be(Book);
    }

    [Fact]
    public async Task Get_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<BookDetailsDto>.NotFound("Книгу не знайдено."));

        var result = await controller.Get(42, Ct);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_Success_ReturnsCreated()
    {
        CreateBookDto? sent = null;
        service.Setup(s => s.CreateAsync(It.IsAny<CreateBookDto>(), Ct))
            .Callback<CreateBookDto, CancellationToken>((dto, _) => sent = dto)
            .ReturnsAsync(Result<int>.Success(1));
        service.Setup(s => s.GetByIdAsync(1, Ct)).ReturnsAsync(Result<BookDetailsDto>.Success(Book));

        var result = await controller.Create(Request, Ct);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.RouteValues!["id"].Should().Be(1);
        created.Value.Should().Be(Book);
        sent!.Isbn.Should().Be(Request.Isbn);
        sent.Cover.Should().BeNull();
    }

    [Fact]
    public async Task Create_ValidationError_ReturnsBadRequest()
    {
        service.Setup(s => s.CreateAsync(It.IsAny<CreateBookDto>(), Ct))
            .ReturnsAsync(Result<int>.Invalid(ControllerTestExtensions.Errors("Isbn", "Книга з таким ISBN уже є.")));

        var result = await controller.Create(Request, Ct);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>().Which.Errors.Should().ContainKey("Isbn");
    }

    [Fact]
    public async Task Update_Success_ReturnsNoContent()
    {
        service.Setup(s => s.UpdateAsync(It.Is<UpdateBookDto>(d => d.Id == 1 && d.Title == "1984"), Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Update(1, Request, Ct);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateBookDto>(), Ct)).ReturnsAsync(Result.NotFound("Книгу не знайдено."));

        var result = await controller.Update(42, Request, Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_Success_ReturnsNoContent()
    {
        service.Setup(s => s.DeleteAsync(1, Ct)).ReturnsAsync(Result.Success());

        var result = await controller.Delete(1, Ct);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_Missing_ReturnsNotFound()
    {
        service.Setup(s => s.DeleteAsync(42, Ct)).ReturnsAsync(Result.NotFound("Книгу не знайдено."));

        var result = await controller.Delete(42, Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UploadCover_Success_KeepsBookFieldsAndPassesFile()
    {
        UpdateBookDto? sent = null;
        service.Setup(s => s.GetByIdAsync(1, Ct)).ReturnsAsync(Result<BookDetailsDto>.Success(Book));
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateBookDto>(), Ct))
            .Callback<UpdateBookDto, CancellationToken>((dto, _) => sent = dto)
            .ReturnsAsync(Result.Success());

        var result = await controller.UploadCover(1, CoverFile("cover.png"), Ct);

        result.Should().BeOfType<NoContentResult>();
        sent!.Title.Should().Be(Book.Title);
        sent.Isbn.Should().Be(Book.Isbn);
        sent.Cover!.FileName.Should().Be("cover.png");
    }

    [Fact]
    public async Task UploadCover_MissingBook_ReturnsNotFound()
    {
        service.Setup(s => s.GetByIdAsync(42, Ct)).ReturnsAsync(Result<BookDetailsDto>.NotFound("Книгу не знайдено."));

        var result = await controller.UploadCover(42, CoverFile("cover.png"), Ct);

        result.Should().BeOfType<NotFoundObjectResult>();
        service.Verify(s => s.UpdateAsync(It.IsAny<UpdateBookDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadCover_NoFile_ReturnsBadRequest()
    {
        service.Setup(s => s.GetByIdAsync(1, Ct)).ReturnsAsync(Result<BookDetailsDto>.Success(Book));

        var result = await controller.UploadCover(1, null, Ct);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>().Which.Errors.Should().ContainKey("file");
    }

    [Fact]
    public async Task UploadCover_InvalidFormat_ReturnsBadRequest()
    {
        service.Setup(s => s.GetByIdAsync(1, Ct)).ReturnsAsync(Result<BookDetailsDto>.Success(Book));
        service.Setup(s => s.UpdateAsync(It.IsAny<UpdateBookDto>(), Ct))
            .ReturnsAsync(Result.Invalid(ControllerTestExtensions.Errors("Cover", "Обкладинка має бути у форматі JPG, PNG або WEBP.")));

        var result = await controller.UploadCover(1, CoverFile("cover.gif"), Ct);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    private static FormFile CoverFile(string name)
    {
        var stream = new MemoryStream([1, 2, 3]);
        return new FormFile(stream, 0, stream.Length, "file", name);
    }
}
