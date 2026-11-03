using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bookshelf.Web.Infrastructure;

public class OutputCacheInvalidationInterceptor(IOutputCacheStore store) : SaveChangesInterceptor
{
    private readonly ConditionalWeakTable<DbContext, HashSet<string>> pendingTags = new();

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            var tags = context.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .SelectMany(e => ApiOutputCache.TagsFor(e.Metadata.ClrType))
                .ToHashSet();
            pendingTags.AddOrUpdate(context, tags);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context && pendingTags.TryGetValue(context, out var tags))
        {
            pendingTags.Remove(context);
            foreach (var tag in tags)
            {
                await store.EvictByTagAsync(tag, CancellationToken.None);
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            pendingTags.Remove(context);
        }

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
}
