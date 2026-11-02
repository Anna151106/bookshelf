using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
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
        var rows = await db.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => new { Genre = g, BookCount = g.Books.Count })
            .ToListAsync(cancellationToken);

        return rows.Select(r => r.Genre.ToListDto(r.BookCount)).ToList();
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

    private const string NotFoundMessage = "Жанр не знайдено.";
}
