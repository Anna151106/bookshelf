using System.ComponentModel.DataAnnotations;
using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Books;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookshelf.Web.ViewModels;

public class BookFormViewModel
{
    [Display(Name = "Назва")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Рік видання")]
    public int? PublishedYear { get; set; }

    [Display(Name = "ISBN")]
    public string Isbn { get; set; } = string.Empty;

    [Display(Name = "Кількість сторінок")]
    public int? Pages { get; set; }

    [Display(Name = "Автор")]
    public int? AuthorId { get; set; }

    [Display(Name = "Жанр")]
    public int? GenreId { get; set; }

    [Display(Name = "Обкладинка")]
    public IFormFile? Cover { get; set; }

    [ValidateNever]
    public string? CurrentCoverPath { get; set; }

    [ValidateNever]
    public string? AuthorName { get; set; }

    [ValidateNever]
    public string? GenreName { get; set; }

    public static BookFormViewModel FromDetails(BookDetailsDto book) => new()
    {
        Title = book.Title,
        Description = book.Description,
        PublishedYear = book.PublishedYear,
        Isbn = book.Isbn,
        Pages = book.Pages,
        AuthorId = book.AuthorId,
        GenreId = book.GenreId,
        AuthorName = book.AuthorName,
        GenreName = book.GenreName,
        CurrentCoverPath = book.CoverPath,
    };

    public CreateBookDto ToCreateDto(FileUpload? cover) => new()
    {
        Title = Title,
        Description = Description,
        PublishedYear = PublishedYear ?? 0,
        Isbn = Isbn,
        Pages = Pages ?? 0,
        AuthorId = AuthorId ?? 0,
        GenreId = GenreId ?? 0,
        Cover = cover,
    };

    public UpdateBookDto ToUpdateDto(int id, FileUpload? cover) => new()
    {
        Id = id,
        Title = Title,
        Description = Description,
        PublishedYear = PublishedYear ?? 0,
        Isbn = Isbn,
        Pages = Pages ?? 0,
        AuthorId = AuthorId ?? 0,
        GenreId = GenreId ?? 0,
        Cover = cover,
    };
}
