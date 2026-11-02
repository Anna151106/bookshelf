using Bookshelf.Application.Reviews;
using Bookshelf.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers.Api;

[ApiController]
[Route("api/reviews")]
public class ReviewsApiController(IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ReviewDto>> GetAll(CancellationToken cancellationToken)
    {
        return await reviewService.GetAllAsync(cancellationToken: cancellationToken);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        return result.Value!;
    }

    [HttpPost]
    [ProducesResponseType<ReviewDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await reviewService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ToProblem();
        }

        var created = await reviewService.GetByIdAsync(result.Value, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Value }, created.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await reviewService.UpdateAsync(dto with { Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
