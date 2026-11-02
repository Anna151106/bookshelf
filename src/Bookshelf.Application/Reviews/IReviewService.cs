using Bookshelf.Application.Common;

namespace Bookshelf.Application.Reviews;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetAllAsync(int? bookId = null, CancellationToken cancellationToken = default);

    Task<Result<ReviewDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<int>> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(UpdateReviewDto dto, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
