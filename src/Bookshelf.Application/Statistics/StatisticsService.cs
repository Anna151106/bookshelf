using Bookshelf.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Statistics;

public class StatisticsService(IAppDbContext db) : IStatisticsService
{
    public async Task<StatisticsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var byGenre = await db.Genres
            .AsNoTracking()
            .Where(g => g.Books.Any())
            .OrderBy(g => g.Name)
            .Select(g => new GenreBookCountDto(g.Name, g.Books.Count))
            .ToListAsync(cancellationToken);

        var byAuthor = await db.Reviews
            .AsNoTracking()
            .GroupBy(r => new { r.Book.AuthorId, r.Book.Author.FullName })
            .Select(g => new { g.Key.FullName, Average = g.Average(r => (double)r.Rating) })
            .OrderBy(r => r.FullName)
            .ToListAsync(cancellationToken);

        var byDecade = await db.Books
            .AsNoTracking()
            .GroupBy(b => b.PublishedYear / 10 * 10)
            .OrderBy(g => g.Key)
            .Select(g => new DecadeBookCountDto(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        return new StatisticsDto(
            byGenre,
            byAuthor.Select(r => new AuthorRatingDto(r.FullName, Math.Round(r.Average, 2))).ToList(),
            byDecade);
    }
}
