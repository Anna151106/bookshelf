using Bookshelf.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Libraries;

public class LibraryService(IAppDbContext db) : ILibraryService
{
    public async Task<IReadOnlyList<LibraryMapPointDto>> GetForMapAsync(LibraryMapFilter filter, CancellationToken cancellationToken = default)
    {
        var bookId = filter.BookId;
        var authorId = filter.AuthorId;
        var genreId = filter.GenreId;
        var district = string.IsNullOrWhiteSpace(filter.District) ? null : filter.District.Trim();
        var hasBookFilter = bookId is not null || authorId is not null || genreId is not null;

        var query = db.Libraries.AsNoTracking();

        if (district is not null)
        {
            query = query.Where(l => l.District == district);
        }

        if (hasBookFilter)
        {
            query = query.Where(l => l.Books.Any(b =>
                (bookId == null || b.Id == bookId) &&
                (authorId == null || b.AuthorId == authorId) &&
                (genreId == null || b.GenreId == genreId)));
        }

        var rows = await query
            .OrderBy(l => l.Name)
            .ThenBy(l => l.Id)
            .Select(l => new
            {
                l.Id,
                l.Name,
                l.Address,
                l.District,
                l.Latitude,
                l.Longitude,
                Books = l.Books
                    .Where(b =>
                        (bookId == null || b.Id == bookId) &&
                        (authorId == null || b.AuthorId == authorId) &&
                        (genreId == null || b.GenreId == genreId))
                    .OrderBy(b => b.Title)
                    .Select(b => new LibraryBookDto(b.Id, b.Title))
                    .ToList(),
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new LibraryMapPointDto(r.Id, r.Name, r.Address, r.District, r.Latitude, r.Longitude, r.Books.Count, r.Books))
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetDistrictsAsync(CancellationToken cancellationToken = default) =>
        await db.Libraries
            .AsNoTracking()
            .Select(l => l.District)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(cancellationToken);
}
