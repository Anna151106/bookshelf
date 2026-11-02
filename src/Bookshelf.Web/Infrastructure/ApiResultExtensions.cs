using Bookshelf.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Infrastructure;

public static class ApiResultExtensions
{
    public static ActionResult ToProblem(this Result result) => result.Status switch
    {
        ResultStatus.NotFound => new NotFoundObjectResult(Problem(StatusCodes.Status404NotFound, "Не знайдено.", result.Error)),
        ResultStatus.Conflict => new ConflictObjectResult(Problem(StatusCodes.Status409Conflict, "Конфлікт.", result.Error)),
        ResultStatus.ValidationError => ValidationProblem(result.ValidationErrors),
        _ => throw new InvalidOperationException($"Результат {result.Status} не є помилкою."),
    };

    public static BadRequestObjectResult ValidationProblem(IReadOnlyDictionary<string, string[]> errors) =>
        new(new ValidationProblemDetails(errors.ToDictionary())
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Помилки валідації.",
        });

    private static ProblemDetails Problem(int status, string title, string? detail) =>
        new() { Status = status, Title = title, Detail = detail };
}
