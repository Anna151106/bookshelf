using Bookshelf.Application.Common;

namespace Bookshelf.Application.Genres;

public interface IGenreService
{
    Task<IReadOnlyList<GenreListDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PagedList<GenreListDto>> GetPageAsync(int skip, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LookupItemDto>> SearchAsync(string? query, CancellationToken cancellationToken = default);

    Task<Result<GenreDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<int>> CreateAsync(CreateGenreDto dto, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(UpdateGenreDto dto, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
