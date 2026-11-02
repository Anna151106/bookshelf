using Bookshelf.Application.Abstractions;
using Bookshelf.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Genres;

public abstract class GenreInputValidator<T> : AbstractValidator<T>
    where T : IGenreInput
{
    protected GenreInputValidator(IAppDbContext db, Func<T, int?> getId)
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Вкажіть назву жанру.")
            .MaximumLength(Genre.NameMaxLength).WithMessage($"Назва задовга (максимум {Genre.NameMaxLength} символів).")
            .MustAsync(async (dto, name, cancellationToken) =>
            {
                var id = getId(dto);
                var normalized = name.Trim().ToLower();
                return !await db.Genres.AnyAsync(g => g.Name.ToLower() == normalized && g.Id != id, cancellationToken);
            })
            .WithMessage("Такий жанр уже є.");
    }
}

public class CreateGenreDtoValidator(IAppDbContext db) : GenreInputValidator<CreateGenreDto>(db, _ => null);

public class UpdateGenreDtoValidator(IAppDbContext db) : GenreInputValidator<UpdateGenreDto>(db, dto => dto.Id);
