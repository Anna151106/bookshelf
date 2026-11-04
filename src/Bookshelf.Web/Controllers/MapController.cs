using Bookshelf.Application.Books;
using Bookshelf.Application.Libraries;
using Bookshelf.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshelf.Web.Controllers;

public class MapController(ILibraryService libraryService, IBookService bookService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var books = await bookService.GetAllAsync(cancellationToken: cancellationToken);
        var districts = await libraryService.GetDistrictsAsync(cancellationToken);

        return View(new MapViewModel
        {
            Books = books.Select(b => new SelectListItem(b.Title, b.Id.ToString())).ToList(),
            Districts = districts.Select(d => new SelectListItem(d, d)).ToList(),
        });
    }
}
