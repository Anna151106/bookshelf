using System.ComponentModel.DataAnnotations;
using Bookshelf.Application.Reviews;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookshelf.Web.ViewModels;

public class ReviewFormViewModel
{
    [Display(Name = "Книга")]
    public int BookId { get; set; }

    [ValidateNever]
    public string? BookTitle { get; set; }

    [ValidateNever]
    public bool CanChooseBook { get; set; }

    [Display(Name = "Ваше ім'я")]
    public string ReaderName { get; set; } = string.Empty;

    [Display(Name = "Оцінка")]
    public int? Rating { get; set; }

    [Display(Name = "Відгук")]
    public string? Text { get; set; }

    public static ReviewFormViewModel FromDetails(ReviewDto review) => new()
    {
        BookId = review.BookId,
        BookTitle = review.BookTitle,
        ReaderName = review.ReaderName,
        Rating = review.Rating,
        Text = review.Text,
    };

    public CreateReviewDto ToCreateDto() => new()
    {
        BookId = BookId,
        ReaderName = ReaderName,
        Rating = Rating ?? 0,
        Text = Text,
    };

    public UpdateReviewDto ToUpdateDto(int id) => new()
    {
        Id = id,
        ReaderName = ReaderName,
        Rating = Rating ?? 0,
        Text = Text,
    };
}
