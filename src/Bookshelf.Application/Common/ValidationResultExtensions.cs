using FluentValidation.Results;

namespace Bookshelf.Application.Common;

public static class ValidationResultExtensions
{
    public static IReadOnlyDictionary<string, string[]> ToErrorDictionary(this ValidationResult result) =>
        result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

    public static IReadOnlyDictionary<string, string[]> SingleError(string propertyName, string message) =>
        new Dictionary<string, string[]> { [propertyName] = [message] };
}
