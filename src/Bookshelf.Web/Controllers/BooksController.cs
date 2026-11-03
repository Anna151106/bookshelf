using Bookshelf.Application.Abstractions;
using Bookshelf.Application.Authors;
using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Application.Genres;
using Bookshelf.Web.Infrastructure;
using Bookshelf.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Bookshelf.Web.Controllers;

public class BooksController(
    IBookService bookService,
    IAuthorService authorService,
    IGenreService genreService,
    IOptions<LibraryContactsOptions> contacts) : Controller
{
    public async Task<IActionResult> Index(int? genreId, int? authorId, string? search, CancellationToken cancellationToken)
    {
        var filter = new BookFilter { GenreId = genreId, AuthorId = authorId, Search = search };

        return View(new BookIndexViewModel
        {
            Books = await bookService.GetAllAsync(filter, cancellationToken),
            GenreId = genreId,
            AuthorId = authorId,
            Search = search,
            Genres = await GenreOptionsAsync(cancellationToken),
            Authors = await AuthorOptionsAsync(cancellationToken),
        });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await bookService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        var book = result.Value!;
        var author = await authorService.GetByIdAsync(book.AuthorId, cancellationToken);

        return View(new BookDetailsViewModel
        {
            Book = book,
            Author = author.Value,
            Contacts = contacts.Value,
            NewReview = new ReviewFormViewModel { BookId = id },
        });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new BookFormViewModel();
        await FillOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var cover = ToFileUpload(model.Cover);
        await using var coverStream = cover?.Content;

        var result = await bookService.CreateAsync(model.ToCreateDto(cover), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddErrors(result);
            await FillOptionsAsync(model, cancellationToken);
            return View(model);
        }

        this.SetSuccess("Книгу додано.");
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await bookService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        var model = BookFormViewModel.FromDetails(result.Value!);
        await FillOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await FillOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var cover = ToFileUpload(model.Cover);
        await using var coverStream = cover?.Content;

        var result = await bookService.UpdateAsync(model.ToUpdateDto(id, cover), cancellationToken);
        switch (result.Status)
        {
            case ResultStatus.NotFound:
                return NotFound();
            case ResultStatus.ValidationError:
                ModelState.AddErrors(result);
                await FillOptionsAsync(model, cancellationToken);
                return View(model);
        }

        this.SetSuccess("Зміни збережено.");
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await bookService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await bookService.DeleteAsync(id, cancellationToken);
        if (result.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        this.SetSuccess("Книгу видалено.");
        return RedirectToAction(nameof(Index));
    }

    private static FileUpload? ToFileUpload(IFormFile? file) =>
        file is { Length: > 0 } ? new FileUpload(file.OpenReadStream(), file.FileName) : null;

    private async Task FillOptionsAsync(BookFormViewModel model, CancellationToken cancellationToken)
    {
        model.Authors = await AuthorOptionsAsync(cancellationToken);
        model.Genres = await GenreOptionsAsync(cancellationToken);
    }

    private async Task<IEnumerable<SelectListItem>> AuthorOptionsAsync(CancellationToken cancellationToken) =>
        (await authorService.GetAllAsync(cancellationToken))
            .Select(a => new SelectListItem(a.FullName, a.Id.ToString()))
            .ToList();

    private async Task<IEnumerable<SelectListItem>> GenreOptionsAsync(CancellationToken cancellationToken) =>
        (await genreService.GetAllAsync(cancellationToken))
            .Select(g => new SelectListItem(g.Name, g.Id.ToString()))
            .ToList();
}
