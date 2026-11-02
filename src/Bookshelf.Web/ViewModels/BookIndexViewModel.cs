using Bookshelf.Application.Books;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshelf.Web.ViewModels;

public class BookIndexViewModel
{
    public IReadOnlyList<BookListDto> Books { get; init; } = [];

    public int? GenreId { get; init; }

    public int? AuthorId { get; init; }

    public string? Search { get; init; }

    public IEnumerable<SelectListItem> Genres { get; init; } = [];

    public IEnumerable<SelectListItem> Authors { get; init; } = [];

    public bool IsFiltered => GenreId is not null || AuthorId is not null || !string.IsNullOrWhiteSpace(Search);
}
