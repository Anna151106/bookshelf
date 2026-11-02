using System.ComponentModel.DataAnnotations;
using Bookshelf.Application.Reviews;

namespace Bookshelf.Web.ViewModels;

public class ReviewFormViewModel
{
    public int BookId { get; set; }

    [Display(Name = "Ваше ім'я")]
    public string ReaderName { get; set; } = string.Empty;

    [Display(Name = "Оцінка")]
    public int? Rating { get; set; }

    [Display(Name = "Відгук")]
    public string? Text { get; set; }

    public static ReviewFormViewModel FromDetails(ReviewDto review) => new()
    {
        BookId = review.BookId,
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
