using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Books;

public class BookService(
    IAppDbContext db,
    IFileStorage fileStorage,
    IValidator<CreateBookDto> createValidator,
    IValidator<UpdateBookDto> updateValidator) : IBookService
{
    public async Task<IReadOnlyList<BookListDto>> GetAllAsync(BookFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .AsQueryable();

        if (filter?.GenreId is { } genreId)
        {
            query = query.Where(b => b.GenreId == genreId);
        }

        if (filter?.AuthorId is { } authorId)
        {
            query = query.Where(b => b.AuthorId == authorId);
        }

        if (!string.IsNullOrWhiteSpace(filter?.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(search));
        }

        var books = await query.OrderBy(b => b.Title).ToListAsync(cancellationToken);
        return books.Select(b => b.ToListDto()).ToList();
    }

    public async Task<PagedList<BookListDto>> GetPageAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var page = await db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .OrderBy(b => b.Title)
            .ThenBy(b => b.Id)
            .ToPagedListAsync(skip, limit, cancellationToken);

        return page.Map(b => b.ToListDto());
    }

    public async Task<IReadOnlyList<LookupItemDto>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var term = query.Trim().ToLower();
        return await db.Books
            .AsNoTracking()
            .Where(b => b.Title.ToLower().Contains(term))
            .OrderBy(b => b.Title)
            .ThenBy(b => b.Id)
            .Take(Lookup.MaxResults)
            .Select(b => new LookupItemDto(b.Id, b.Title))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<BookDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        return book is null
            ? Result<BookDetailsDto>.NotFound(NotFoundMessage)
            : Result<BookDetailsDto>.Success(book.ToDetailsDto());
    }

    public async Task<Result<int>> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Invalid(validation.ToErrorDictionary());
        }

        var book = dto.ToEntity();

        if (dto.Cover is not null)
        {
            var saved = await SaveCoverAsync(dto.Cover, cancellationToken);
            if (!saved.IsSuccess)
            {
                return Result<int>.Invalid(saved.ValidationErrors);
            }

            book.CoverPath = saved.Value;
        }

        db.Books.Add(book);
        await SaveChangesOrDiscardCoverAsync(book.CoverPath, cancellationToken);

        return Result<int>.Success(book.Id);
    }

    public async Task<Result> UpdateAsync(UpdateBookDto dto, CancellationToken cancellationToken = default)
    {
        var book = await db.Books.FindAsync([dto.Id], cancellationToken);
        if (book is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        var validation = await updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Invalid(validation.ToErrorDictionary());
        }

        string? newCoverPath = null;
        if (dto.Cover is not null)
        {
            var saved = await SaveCoverAsync(dto.Cover, cancellationToken);
            if (!saved.IsSuccess)
            {
                return Result.Invalid(saved.ValidationErrors);
            }

            newCoverPath = saved.Value;
        }

        var oldCoverPath = book.CoverPath;
        dto.ApplyTo(book);
        if (newCoverPath is not null)
        {
            book.CoverPath = newCoverPath;
        }

        await SaveChangesOrDiscardCoverAsync(newCoverPath, cancellationToken);

        // стару обкладинку видаляємо тільки після збереження, щоб у БД не лишилось посилання на неіснуючий файл
        if (newCoverPath is not null && oldCoverPath is not null)
        {
            await fileStorage.DeleteAsync(oldCoverPath, cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);

        if (book.CoverPath is not null)
        {
            await fileStorage.DeleteAsync(book.CoverPath, cancellationToken);
        }

        return Result.Success();
    }

    private async Task<Result<string>> SaveCoverAsync(FileUpload cover, CancellationToken cancellationToken)
    {
        try
        {
            var path = await fileStorage.SaveAsync(cover.Content, cover.FileName, cancellationToken);
            return Result<string>.Success(path);
        }
        catch (FileStorageException ex)
        {
            return Result<string>.Invalid(ValidationResultExtensions.SingleError(nameof(IBookInput.Cover), ex.Message));
        }
    }

    // якщо запис у БД не вдався, щойно завантажений файл нікому не потрібен
    private async Task SaveChangesOrDiscardCoverAsync(string? uploadedCoverPath, CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch when (uploadedCoverPath is not null)
        {
            await fileStorage.DeleteAsync(uploadedCoverPath, CancellationToken.None);
            throw;
        }
    }

    private const string NotFoundMessage = "Книгу не знайдено.";
}
