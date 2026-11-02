using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Common;
using Bookshelf.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Books;

public abstract class BookInputValidator<T> : AbstractValidator<T>
    where T : IBookInput
{
    protected BookInputValidator(IAppDbContext db, TimeProvider timeProvider, Func<T, int?> getId)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Вкажіть назву книги.")
            .MaximumLength(Book.TitleMaxLength).WithMessage($"Назва задовга (максимум {Book.TitleMaxLength} символів).");

        RuleFor(x => x.Description)
            .MaximumLength(Book.DescriptionMaxLength).WithMessage($"Опис задовгий (максимум {Book.DescriptionMaxLength} символів).");

        RuleFor(x => x.PublishedYear)
            .Must(year => year > 0 && year <= timeProvider.GetUtcNow().Year)
            .WithMessage("Рік видання не може бути більшим за поточний.");

        RuleFor(x => x.Pages)
            .GreaterThan(0).WithMessage("Кількість сторінок має бути більше нуля.");

        RuleFor(x => x.Isbn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Вкажіть ISBN.")
            .Must(IsbnFormat.IsValid).WithMessage("Неправильний ISBN.")
            .MustAsync(async (dto, isbn, cancellationToken) =>
            {
                var id = getId(dto);
                var normalized = IsbnFormat.Normalize(isbn);
                return !await db.Books.AnyAsync(b => b.Isbn == normalized && b.Id != id, cancellationToken);
            })
            .WithMessage("Книга з таким ISBN уже є.");

        RuleFor(x => x.AuthorId)
            .MustAsync((authorId, cancellationToken) => db.Authors.AnyAsync(a => a.Id == authorId, cancellationToken))
            .WithMessage("Оберіть автора.");

        RuleFor(x => x.GenreId)
            .MustAsync((genreId, cancellationToken) => db.Genres.AnyAsync(g => g.Id == genreId, cancellationToken))
            .WithMessage("Оберіть жанр.");
    }
}

public class CreateBookDtoValidator(IAppDbContext db, TimeProvider timeProvider)
    : BookInputValidator<CreateBookDto>(db, timeProvider, _ => null);

public class UpdateBookDtoValidator(IAppDbContext db, TimeProvider timeProvider)
    : BookInputValidator<UpdateBookDto>(db, timeProvider, dto => dto.Id);
