using Bookshelf.Domain.Entities;
using FluentValidation;

namespace Bookshelf.Application.Authors;

public abstract class AuthorInputValidator<T> : AbstractValidator<T>
    where T : IAuthorInput
{
    protected AuthorInputValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Вкажіть ім'я автора.")
            .MaximumLength(Author.FullNameMaxLength).WithMessage($"Ім'я задовге (максимум {Author.FullNameMaxLength} символів).");

        RuleFor(x => x.Biography)
            .MaximumLength(Author.BiographyMaxLength).WithMessage($"Біографія задовга (максимум {Author.BiographyMaxLength} символів).");

        RuleFor(x => x.Country)
            .MaximumLength(Author.CountryMaxLength).WithMessage($"Назва країни задовга (максимум {Author.CountryMaxLength} символів).");

        RuleFor(x => x.BirthYear)
            .Must(year => year is null || (year > 0 && year <= timeProvider.GetUtcNow().Year))
            .WithMessage("Неправильний рік народження.");
    }
}

public class CreateAuthorDtoValidator(TimeProvider timeProvider) : AuthorInputValidator<CreateAuthorDto>(timeProvider);

public class UpdateAuthorDtoValidator(TimeProvider timeProvider) : AuthorInputValidator<UpdateAuthorDto>(timeProvider);
