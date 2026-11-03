using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Books;
using Bookshelf.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Bookshelf.Web.Controllers.Api;

[ApiController]
[OutputCache(PolicyName = ApiOutputCache.Books)]
[Route("api/books")]
public class BooksApiController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<PagedResponse<BookListDto>> GetAll([FromQuery] PageQuery query, CancellationToken cancellationToken)
    {
        var page = await bookService.GetPageAsync(query.Skip, query.Limit, cancellationToken);
        return PagedResponse.From(page, query, Request);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDetailsDto>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await bookService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        return result.Value!;
    }

    [HttpPost]
    [ProducesResponseType<BookDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDetailsDto>> Create(BookRequest request, CancellationToken cancellationToken)
    {
        var result = await bookService.CreateAsync(request.ToCreateDto(), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        var created = await bookService.GetByIdAsync(result.Value, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Value }, created.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, BookRequest request, CancellationToken cancellationToken)
    {
        var result = await bookService.UpdateAsync(request.ToUpdateDto(id), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await bookService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("{id:int}/cover")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UploadCover(int id, IFormFile? file, CancellationToken cancellationToken)
    {
        var existing = await bookService.GetByIdAsync(id, cancellationToken);
        if (!existing.IsSuccess)
        {
            return existing.ToProblem();
        }

        if (file is not { Length: > 0 })
        {
            return ApiResultExtensions.ValidationProblem(
                new Dictionary<string, string[]> { [nameof(file)] = ["Додайте файл обкладинки."] });
        }

        var book = existing.Value!;
        await using var content = file.OpenReadStream();
        var dto = new UpdateBookDto
        {
            Id = id,
            Title = book.Title,
            Description = book.Description,
            PublishedYear = book.PublishedYear,
            Isbn = book.Isbn,
            Pages = book.Pages,
            AuthorId = book.AuthorId,
            GenreId = book.GenreId,
            Cover = new FileUpload(content, file.FileName),
        };

        var result = await bookService.UpdateAsync(dto, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
