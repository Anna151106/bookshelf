using Bookshelf.Application.Books;

namespace Bookshelf.Application.Authors;

public record AuthorListDto(int Id, string FullName, string? Country, int? BirthYear, int BookCount);

public record AuthorDetailsDto(
    int Id,
    string FullName,
    string? Biography,
    int? BirthYear,
    string? Country,
    IReadOnlyList<BookSummaryDto> Books);

public interface IAuthorInput
{
    string FullName { get; }

    string? Biography { get; }

    int? BirthYear { get; }

    string? Country { get; }
}

public record CreateAuthorDto : IAuthorInput
{
    public string FullName { get; init; } = string.Empty;

    public string? Biography { get; init; }

    public int? BirthYear { get; init; }

    public string? Country { get; init; }
}

public record UpdateAuthorDto : IAuthorInput
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string? Biography { get; init; }

    public int? BirthYear { get; init; }

    public string? Country { get; init; }
}
