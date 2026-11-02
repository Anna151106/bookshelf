using Bookshelf.Application.Books;
using Bookshelf.Domain.Entities;

namespace Bookshelf.Application.Genres;

public static class GenreMappings
{
    public static GenreListDto ToListDto(this Genre genre, int bookCount) =>
        new(genre.Id, genre.Name, bookCount);

    public static GenreDetailsDto ToDetailsDto(this Genre genre) =>
        new(genre.Id, genre.Name, genre.Books.OrderBy(b => b.Title).Select(b => b.ToSummaryDto()).ToList());

    public static Genre ToEntity(this CreateGenreDto dto)
    {
        var genre = new Genre();
        dto.ApplyTo(genre);
        return genre;
    }

    public static void ApplyTo(this IGenreInput input, Genre genre)
    {
        genre.Name = input.Name.Trim();
    }
}
