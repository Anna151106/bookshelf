using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Domain.Entities;

namespace Bookshelf.Application.Authors;

public static class AuthorMappings
{
    public static AuthorListDto ToListDto(this Author author, int bookCount) =>
        new(author.Id, author.FullName, author.Country, author.BirthYear, bookCount);

    public static AuthorDetailsDto ToDetailsDto(this Author author) =>
        new(
            author.Id,
            author.FullName,
            author.Biography,
            author.BirthYear,
            author.Country,
            author.Books.OrderBy(b => b.Title).Select(b => b.ToSummaryDto()).ToList());

    public static Author ToEntity(this CreateAuthorDto dto)
    {
        var author = new Author();
        dto.ApplyTo(author);
        return author;
    }

    public static void ApplyTo(this IAuthorInput input, Author author)
    {
        author.FullName = input.FullName.Trim();
        author.Biography = input.Biography.NullIfWhiteSpace();
        author.BirthYear = input.BirthYear;
        author.Country = input.Country.NullIfWhiteSpace();
    }
}
