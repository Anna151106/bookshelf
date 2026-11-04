using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
using Bookshelf.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Genres;

public class GenreService(
    IAppDbContext db,
    IValidator<CreateGenreDto> createValidator,
    IValidator<UpdateGenreDto> updateValidator) : IGenreService
{
    public async Task<IReadOnlyList<GenreListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await ListQuery().ToListAsync(cancellationToken);
        return rows.Select(r => r.Genre.ToListDto(r.BookCount)).ToList();
    }

    public async Task<PagedList<GenreListDto>> GetPageAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var page = await ListQuery().ToPagedListAsync(skip, limit, cancellationToken);
        return page.Map(r => r.Genre.ToListDto(r.BookCount));
    }

    public async Task<IReadOnlyList<LookupItemDto>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var term = query.Trim().ToLower();
        return await db.Genres
            .AsNoTracking()
            .Where(g => g.Name.ToLower().Contains(term))
            .OrderBy(g => g.Name)
            .ThenBy(g => g.Id)
            .Take(Lookup.MaxResults)
            .Select(g => new LookupItemDto(g.Id, g.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<GenreDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var genre = await db.Genres
            .AsNoTracking()
            .Include(g => g.Books)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        return genre is null
            ? Result<GenreDetailsDto>.NotFound(NotFoundMessage)
            : Result<GenreDetailsDto>.Success(genre.ToDetailsDto());
    }

    public async Task<Result<int>> CreateAsync(CreateGenreDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Invalid(validation.ToErrorDictionary());
        }

        var genre = dto.ToEntity();
        db.Genres.Add(genre);
        await db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(genre.Id);
    }

    public async Task<Result> UpdateAsync(UpdateGenreDto dto, CancellationToken cancellationToken = default)
    {
        var genre = await db.Genres.FindAsync([dto.Id], cancellationToken);
        if (genre is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        var validation = await updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Invalid(validation.ToErrorDictionary());
        }

        dto.ApplyTo(genre);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var genre = await db.Genres.FindAsync([id], cancellationToken);
        if (genre is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        if (await db.Books.AnyAsync(b => b.GenreId == id, cancellationToken))
        {
            return Result.Conflict("Не можна видалити жанр, у якому є книги.");
        }

        db.Genres.Remove(genre);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private IQueryable<GenreRow> ListQuery() =>
        db.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ThenBy(g => g.Id)
            .Select(g => new GenreRow(g, g.Books.Count));

    private sealed record GenreRow(Genre Genre, int BookCount);

    private const string NotFoundMessage = "Жанр не знайдено.";
}
