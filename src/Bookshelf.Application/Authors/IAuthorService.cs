using Bookshelf.Application.Common;

namespace Bookshelf.Application.Authors;

public interface IAuthorService
{
    Task<IReadOnlyList<AuthorListDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<AuthorDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<int>> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(UpdateAuthorDto dto, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
