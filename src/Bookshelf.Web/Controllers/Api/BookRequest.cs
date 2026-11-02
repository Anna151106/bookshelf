using Bookshelf.Application.Books;

namespace Bookshelf.Web.Controllers.Api;

public record BookRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int PublishedYear { get; init; }

    public string Isbn { get; init; } = string.Empty;

    public int Pages { get; init; }

    public int AuthorId { get; init; }

    public int GenreId { get; init; }

    public CreateBookDto ToCreateDto() => new()
    {
        Title = Title,
        Description = Description,
        PublishedYear = PublishedYear,
        Isbn = Isbn,
        Pages = Pages,
        AuthorId = AuthorId,
        GenreId = GenreId,
    };

    public UpdateBookDto ToUpdateDto(int id) => new()
    {
        Id = id,
        Title = Title,
        Description = Description,
        PublishedYear = PublishedYear,
        Isbn = Isbn,
        Pages = Pages,
        AuthorId = AuthorId,
        GenreId = GenreId,
    };
}
