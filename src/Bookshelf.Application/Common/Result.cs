namespace Bookshelf.Application.Common;

public enum ResultStatus
{
    Success,
    NotFound,
    ValidationError,
    Conflict,
}

public class Result
{
    private static readonly IReadOnlyDictionary<string, string[]> NoErrors = new Dictionary<string, string[]>();

    protected Result(ResultStatus status, string? error, IReadOnlyDictionary<string, string[]>? validationErrors)
    {
        Status = status;
        Error = error;
        ValidationErrors = validationErrors ?? NoErrors;
    }

    public ResultStatus Status { get; }

    public bool IsSuccess => Status == ResultStatus.Success;

    public string? Error { get; }

    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

    public static Result Success() => new(ResultStatus.Success, null, null);

    public static Result NotFound(string error) => new(ResultStatus.NotFound, error, null);

    public static Result Conflict(string error) => new(ResultStatus.Conflict, error, null);

    public static Result Invalid(IReadOnlyDictionary<string, string[]> errors) =>
        new(ResultStatus.ValidationError, null, errors);
}

public sealed class Result<T> : Result
{
    private Result(ResultStatus status, T? value, string? error, IReadOnlyDictionary<string, string[]>? validationErrors)
        : base(status, error, validationErrors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(ResultStatus.Success, value, null, null);

    public static new Result<T> NotFound(string error) => new(ResultStatus.NotFound, default, error, null);

    public static new Result<T> Invalid(IReadOnlyDictionary<string, string[]> errors) =>
        new(ResultStatus.ValidationError, default, null, errors);
}
