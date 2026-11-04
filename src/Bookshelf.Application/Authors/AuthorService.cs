using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
using Bookshelf.Domain.Entities;
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
        var rows = await ListQuery().ToListAsync(cancellationToken);
        return rows.Select(r => r.Author.ToListDto(r.BookCount)).ToList();
    }

    public async Task<PagedList<AuthorListDto>> GetPageAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var page = await ListQuery().ToPagedListAsync(skip, limit, cancellationToken);
        return page.Map(r => r.Author.ToListDto(r.BookCount));
    }

    public async Task<IReadOnlyList<LookupItemDto>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var term = query.Trim().ToLower();
        return await db.Authors
            .AsNoTracking()
            .Where(a => a.FullName.ToLower().Contains(term))
            .OrderBy(a => a.FullName)
            .ThenBy(a => a.Id)
            .Take(Lookup.MaxResults)
            .Select(a => new LookupItemDto(a.Id, a.FullName))
            .ToListAsync(cancellationToken);
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

    private IQueryable<AuthorRow> ListQuery() =>
        db.Authors
            .AsNoTracking()
            .OrderBy(a => a.FullName)
            .ThenBy(a => a.Id)
            .Select(a => new AuthorRow(a, a.Books.Count));

    private sealed record AuthorRow(Author Author, int BookCount);

    private const string NotFoundMessage = "Автора не знайдено.";
}
