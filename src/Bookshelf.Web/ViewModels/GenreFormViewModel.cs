using System.ComponentModel.DataAnnotations;
using Bookshelf.Application.Genres;

namespace Bookshelf.Web.ViewModels;

public class GenreFormViewModel
{
    [Display(Name = "Назва")]
    public string Name { get; set; } = string.Empty;

    public static GenreFormViewModel FromDetails(GenreDetailsDto genre) => new() { Name = genre.Name };

    public CreateGenreDto ToCreateDto() => new() { Name = Name };

    public UpdateGenreDto ToUpdateDto(int id) => new() { Id = id, Name = Name };
}
