using Bookshelf.Application.Common;
using Bookshelf.Application.Reviews;
using Bookshelf.Domain.Entities;

namespace Bookshelf.Application.Books;

public static class BookMappings
{
    public static BookSummaryDto ToSummaryDto(this Book book) =>
        new(book.Id, book.Title, book.PublishedYear);

    public static BookListDto ToListDto(this Book book) =>
        new(
            book.Id,
            book.Title,
            book.PublishedYear,
            book.CoverPath,
            book.AuthorId,
            book.Author.FullName,
            book.GenreId,
            book.Genre.Name);

    public static BookDetailsDto ToDetailsDto(this Book book) =>
        new(
            book.Id,
            book.Title,
            book.Description,
            book.PublishedYear,
            book.Isbn,
            book.Pages,
            book.CoverPath,
            book.AuthorId,
            book.Author.FullName,
            book.GenreId,
            book.Genre.Name,
            book.Reviews.Count == 0 ? null : Math.Round(book.Reviews.Average(r => r.Rating), 1),
            book.Reviews.OrderByDescending(r => r.CreatedAt).Select(r => r.ToDto(book.Title)).ToList());

    public static Book ToEntity(this CreateBookDto dto)
    {
        var book = new Book();
        dto.ApplyTo(book);
        return book;
    }

    public static void ApplyTo(this IBookInput input, Book book)
    {
        book.Title = input.Title.Trim();
        book.Description = input.Description.NullIfWhiteSpace();
        book.PublishedYear = input.PublishedYear;
        book.Isbn = IsbnFormat.Normalize(input.Isbn);
        book.Pages = input.Pages;
        book.AuthorId = input.AuthorId;
        book.GenreId = input.GenreId;
    }
}
