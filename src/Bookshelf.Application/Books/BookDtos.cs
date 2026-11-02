using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Reviews;

namespace Bookshelf.Application.Books;

public record BookSummaryDto(int Id, string Title, int PublishedYear);

public record BookListDto(
    int Id,
    string Title,
    int PublishedYear,
    string? CoverPath,
    int AuthorId,
    string AuthorName,
    int GenreId,
    string GenreName);

public record BookDetailsDto(
    int Id,
    string Title,
    string? Description,
    int PublishedYear,
    string Isbn,
    int Pages,
    string? CoverPath,
    int AuthorId,
    string AuthorName,
    int GenreId,
    string GenreName,
    double? AverageRating,
    IReadOnlyList<ReviewDto> Reviews);

public record BookFilter
{
    public int? GenreId { get; init; }

    public int? AuthorId { get; init; }

    public string? Search { get; init; }
}

public interface IBookInput
{
    string Title { get; }

    string? Description { get; }

    int PublishedYear { get; }

    string Isbn { get; }

    int Pages { get; }

    int AuthorId { get; }

    int GenreId { get; }

    FileUpload? Cover { get; }
}

public record CreateBookDto : IBookInput
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int PublishedYear { get; init; }

    public string Isbn { get; init; } = string.Empty;

    public int Pages { get; init; }

    public int AuthorId { get; init; }

    public int GenreId { get; init; }

    public FileUpload? Cover { get; init; }
}

public record UpdateBookDto : IBookInput
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int PublishedYear { get; init; }

    public string Isbn { get; init; } = string.Empty;

    public int Pages { get; init; }

    public int AuthorId { get; init; }

    public int GenreId { get; init; }

    public FileUpload? Cover { get; init; }
}
