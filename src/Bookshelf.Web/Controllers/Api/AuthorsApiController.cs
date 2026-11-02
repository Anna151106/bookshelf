using Bookshelf.Application.Authors;
using Bookshelf.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers.Api;

[ApiController]
[Route("api/authors")]
public class AuthorsApiController(IAuthorService authorService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<PagedResponse<AuthorListDto>> GetAll([FromQuery] PageQuery query, CancellationToken cancellationToken)
    {
        var page = await authorService.GetPageAsync(query.Skip, query.Limit, cancellationToken);
        return PagedResponse.From(page, query, Request);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDetailsDto>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await authorService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        return result.Value!;
    }

    [HttpPost]
    [ProducesResponseType<AuthorDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorDetailsDto>> Create(CreateAuthorDto dto, CancellationToken cancellationToken)
    {
        var result = await authorService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        var created = await authorService.GetByIdAsync(result.Value, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Value }, created.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, UpdateAuthorDto dto, CancellationToken cancellationToken)
    {
        var result = await authorService.UpdateAsync(dto with { Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await authorService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
