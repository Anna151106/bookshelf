namespace Bookshelf.Application.Libraries;

public interface ILibraryService
{
    Task<IReadOnlyList<LibraryMapPointDto>> GetForMapAsync(LibraryMapFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetDistrictsAsync(CancellationToken cancellationToken = default);
}
