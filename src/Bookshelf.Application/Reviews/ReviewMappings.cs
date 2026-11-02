using Bookshelf.Application.Common;
using Bookshelf.Domain.Entities;

namespace Bookshelf.Application.Reviews;

public static class ReviewMappings
{
    public static ReviewDto ToDto(this Review review, string bookTitle) =>
        new(review.Id, review.BookId, bookTitle, review.ReaderName, review.Rating, review.Text, review.CreatedAt);

    public static Review ToEntity(this CreateReviewDto dto, DateTime createdAt)
    {
        var review = new Review { BookId = dto.BookId, CreatedAt = createdAt };
        dto.ApplyTo(review);
        return review;
    }

    public static void ApplyTo(this IReviewInput input, Review review)
    {
        review.ReaderName = input.ReaderName.Trim();
        review.Rating = input.Rating;
        review.Text = input.Text.NullIfWhiteSpace();
    }
}
