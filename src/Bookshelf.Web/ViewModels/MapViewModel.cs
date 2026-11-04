using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshelf.Web.ViewModels;

public class MapViewModel
{
    public IEnumerable<SelectListItem> Books { get; init; } = [];

    public IEnumerable<SelectListItem> Districts { get; init; } = [];
}
