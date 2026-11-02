using Bookshelf.Application.Genres;
using Bookshelf.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers.Api;

[ApiController]
[Route("api/genres")]
public class GenresApiController(IGenreService genreService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<PagedResponse<GenreListDto>> GetAll([FromQuery] PageQuery query, CancellationToken cancellationToken)
    {
        var page = await genreService.GetPageAsync(query.Skip, query.Limit, cancellationToken);
        return PagedResponse.From(page, query, Request);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDetailsDto>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await genreService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        return result.Value!;
    }

    [HttpPost]
    [ProducesResponseType<GenreDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenreDetailsDto>> Create(CreateGenreDto dto, CancellationToken cancellationToken)
    {
        var result = await genreService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        var created = await genreService.GetByIdAsync(result.Value, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Value }, created.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, UpdateGenreDto dto, CancellationToken cancellationToken)
    {
        var result = await genreService.UpdateAsync(dto with { Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await genreService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
