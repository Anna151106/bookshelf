using Bookshelf.Application.Authors;
using Bookshelf.Application.Common;
using Bookshelf.Web.Infrastructure;
using Bookshelf.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class AuthorsController(IAuthorService authorService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await authorService.GetAllAsync(cancellationToken));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await authorService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    public IActionResult Create()
    {
        return View(new AuthorFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await authorService.CreateAsync(model.ToCreateDto(), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddErrors(result);
            return View(model);
        }

        this.SetSuccess("Автора додано.");
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await authorService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(AuthorFormViewModel.FromDetails(result.Value!)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await authorService.UpdateAsync(model.ToUpdateDto(id), cancellationToken);
        switch (result.Status)
        {
            case ResultStatus.NotFound:
                return NotFound();
            case ResultStatus.ValidationError:
                ModelState.AddErrors(result);
                return View(model);
        }

        this.SetSuccess("Зміни збережено.");
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await authorService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await authorService.DeleteAsync(id, cancellationToken);
        switch (result.Status)
        {
            case ResultStatus.NotFound:
                return NotFound();
            case ResultStatus.Conflict:
                this.SetError(result.Error!);
                return RedirectToAction(nameof(Details), new { id });
        }

        this.SetSuccess("Автора видалено.");
        return RedirectToAction(nameof(Index));
    }
}
