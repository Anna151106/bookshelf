using Bookshelf.Application.Abstractions;
using Bookshelf.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Reviews;

public abstract class ReviewInputValidator<T> : AbstractValidator<T>
    where T : IReviewInput
{
    protected ReviewInputValidator()
    {
        RuleFor(x => x.ReaderName)
            .NotEmpty().WithMessage("Вкажіть своє ім'я.")
            .MaximumLength(Review.ReaderNameMaxLength).WithMessage($"Ім'я задовге (максимум {Review.ReaderNameMaxLength} символів).");

        RuleFor(x => x.Rating)
            .InclusiveBetween(Review.MinRating, Review.MaxRating)
            .WithMessage($"Оцінка має бути від {Review.MinRating} до {Review.MaxRating}.");

        RuleFor(x => x.Text)
            .MaximumLength(Review.TextMaxLength).WithMessage($"Відгук задовгий (максимум {Review.TextMaxLength} символів).");
    }
}

public class CreateReviewDtoValidator : ReviewInputValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator(IAppDbContext db)
    {
        RuleFor(x => x.BookId)
            .MustAsync((bookId, cancellationToken) => db.Books.AnyAsync(b => b.Id == bookId, cancellationToken))
            .WithMessage("Книгу не знайдено.");
    }
}

public class UpdateReviewDtoValidator : ReviewInputValidator<UpdateReviewDto>;
