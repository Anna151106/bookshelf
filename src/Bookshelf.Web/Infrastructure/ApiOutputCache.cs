using Bookshelf.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bookshelf.Web.Infrastructure;

public static class ApiOutputCache
{
    public const string Authors = "authors";
    public const string Genres = "genres";
    public const string Books = "books";
    public const string Reviews = "reviews";

    private static readonly Dictionary<Type, string[]> AffectedTags = new()
    {
        [typeof(Author)] = [Authors, Books],
        [typeof(Genre)] = [Genres, Books],
        [typeof(Book)] = [Books, Authors, Genres, Reviews],
        [typeof(Review)] = [Reviews, Books],
    };

    public static IServiceCollection AddApiOutputCache(this IServiceCollection services, IConfiguration configuration)
    {
        var expiration = TimeSpan.FromSeconds(configuration.GetValue("ApiCache:ExpirationSeconds", 60));

        services.AddOutputCache(options =>
        {
            foreach (var tag in new[] { Authors, Genres, Books, Reviews })
            {
                options.AddPolicy(tag, policy => policy.Expire(expiration).Tag(tag));
            }
        });
        services.AddSingleton<ISaveChangesInterceptor, OutputCacheInvalidationInterceptor>();

        return services;
    }

    public static IEnumerable<string> TagsFor(Type entityType) =>
        AffectedTags.TryGetValue(entityType, out var tags) ? tags : [];
}
