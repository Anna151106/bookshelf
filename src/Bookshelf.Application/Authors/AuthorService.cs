using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Authors;

public class AuthorService(
    IAppDbContext db,
    IValidator<CreateAuthorDto> createValidator,
    IValidator<UpdateAuthorDto> updateValidator) : IAuthorService
{
    public async Task<IReadOnlyList<AuthorListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await db.Authors
            .AsNoTracking()
            .OrderBy(a => a.FullName)
            .Select(a => new { Author = a, BookCount = a.Books.Count })
            .ToListAsync(cancellationToken);

        return rows.Select(r => r.Author.ToListDto(r.BookCount)).ToList();
    }

    public async Task<Result<AuthorDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await db.Authors
            .AsNoTracking()
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return author is null
            ? Result<AuthorDetailsDto>.NotFound(NotFoundMessage)
            : Result<AuthorDetailsDto>.Success(author.ToDetailsDto());
    }

    public async Task<Result<int>> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Invalid(validation.ToErrorDictionary());
        }

        var author = dto.ToEntity();
        db.Authors.Add(author);
        await db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(author.Id);
    }

    public async Task<Result> UpdateAsync(UpdateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = await db.Authors.FindAsync([dto.Id], cancellationToken);
        if (author is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        var validation = await updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Invalid(validation.ToErrorDictionary());
        }

        dto.ApplyTo(author);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await db.Authors.FindAsync([id], cancellationToken);
        if (author is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        if (await db.Books.AnyAsync(b => b.AuthorId == id, cancellationToken))
        {
            return Result.Conflict("Не можна видалити автора, у якого є книги.");
        }

        db.Authors.Remove(author);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private const string NotFoundMessage = "Автора не знайдено.";
}
