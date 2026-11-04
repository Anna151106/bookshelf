namespace Bookshelf.Application.Libraries;

public record LibraryMapFilter
{
    public int? BookId { get; init; }

    public int? AuthorId { get; init; }

    public int? GenreId { get; init; }

    public string? District { get; init; }
}

public record LibraryBookDto(int Id, string Title);

public record LibraryMapPointDto(
    int Id,
    string Name,
    string Address,
    string District,
    double Latitude,
    double Longitude,
    int BookCount,
    IReadOnlyList<LibraryBookDto> Books);
