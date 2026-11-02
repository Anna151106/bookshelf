using System.ComponentModel.DataAnnotations;
using Bookshelf.Application.Authors;

namespace Bookshelf.Web.ViewModels;

public class AuthorFormViewModel
{
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Біографія")]
    public string? Biography { get; set; }

    [Display(Name = "Рік народження")]
    public int? BirthYear { get; set; }

    [Display(Name = "Країна")]
    public string? Country { get; set; }

    public static AuthorFormViewModel FromDetails(AuthorDetailsDto author) => new()
    {
        FullName = author.FullName,
        Biography = author.Biography,
        BirthYear = author.BirthYear,
        Country = author.Country,
    };

    public CreateAuthorDto ToCreateDto() => new()
    {
        FullName = FullName,
        Biography = Biography,
        BirthYear = BirthYear,
        Country = Country,
    };

    public UpdateAuthorDto ToUpdateDto(int id) => new()
    {
        Id = id,
        FullName = FullName,
        Biography = Biography,
        BirthYear = BirthYear,
        Country = Country,
    };
}
