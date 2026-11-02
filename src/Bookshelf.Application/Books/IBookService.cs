using Bookshelf.Application.Common;

namespace Bookshelf.Application.Books;

public interface IBookService
{
    Task<IReadOnlyList<BookListDto>> GetAllAsync(BookFilter? filter = null, CancellationToken cancellationToken = default);

    Task<Result<BookDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<int>> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(UpdateBookDto dto, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
