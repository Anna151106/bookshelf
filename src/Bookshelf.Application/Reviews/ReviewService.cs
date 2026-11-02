using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Reviews;

public class ReviewService(
    IAppDbContext db,
    TimeProvider timeProvider,
    IValidator<CreateReviewDto> createValidator,
    IValidator<UpdateReviewDto> updateValidator) : IReviewService
{
    public async Task<IReadOnlyList<ReviewDto>> GetAllAsync(int? bookId = null, CancellationToken cancellationToken = default)
    {
        var query = db.Reviews.AsNoTracking().Include(r => r.Book).AsQueryable();

        if (bookId is not null)
        {
            query = query.Where(r => r.BookId == bookId);
        }

        var reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);
        return reviews.Select(r => r.ToDto(r.Book.Title)).ToList();
    }

    public async Task<PagedList<ReviewDto>> GetPageAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var page = await db.Reviews
            .AsNoTracking()
            .Include(r => r.Book)
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id)
            .ToPagedListAsync(skip, limit, cancellationToken);

        return page.Map(r => r.ToDto(r.Book.Title));
    }

    public async Task<Result<ReviewDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await db.Reviews
            .AsNoTracking()
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return review is null
            ? Result<ReviewDto>.NotFound(NotFoundMessage)
            : Result<ReviewDto>.Success(review.ToDto(review.Book.Title));
    }

    public async Task<Result<int>> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Invalid(validation.ToErrorDictionary());
        }

        var review = dto.ToEntity(timeProvider.GetUtcNow().UtcDateTime);
        db.Reviews.Add(review);
        await db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(review.Id);
    }

    public async Task<Result> UpdateAsync(UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var review = await db.Reviews.FindAsync([dto.Id], cancellationToken);
        if (review is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        var validation = await updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Invalid(validation.ToErrorDictionary());
        }

        dto.ApplyTo(review);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await db.Reviews.FindAsync([id], cancellationToken);
        if (review is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        db.Reviews.Remove(review);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private const string NotFoundMessage = "Відгук не знайдено.";
}
