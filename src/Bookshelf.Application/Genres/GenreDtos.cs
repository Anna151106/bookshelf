using Bookshelf.Application.Books;

namespace Bookshelf.Application.Genres;

public record GenreListDto(int Id, string Name, int BookCount);

public record GenreDetailsDto(int Id, string Name, IReadOnlyList<BookSummaryDto> Books);

public interface IGenreInput
{
    string Name { get; }
}

public record CreateGenreDto : IGenreInput
{
    public string Name { get; init; } = string.Empty;
}

public record UpdateGenreDto : IGenreInput
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}
