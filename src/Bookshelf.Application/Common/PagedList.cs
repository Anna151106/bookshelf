using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Common;

public record PagedList<T>(IReadOnlyList<T> Items, int Total)
{
    public PagedList<TResult> Map<TResult>(Func<T, TResult> selector) => new(Items.Select(selector).ToList(), Total);
}

public static class PagedListExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query, int skip, int limit, CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(limit).ToListAsync(cancellationToken);
        return new PagedList<T>(items, total);
    }
}
