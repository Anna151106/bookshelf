namespace Bookshelf.Application.Common;

public record LookupItemDto(int Id, string Text);

public static class Lookup
{
    public const int MaxResults = 20;
}
