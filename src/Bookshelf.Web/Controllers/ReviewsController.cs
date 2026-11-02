using Bookshelf.Application.Common;
using Bookshelf.Application.Reviews;
using Bookshelf.Web.Infrastructure;
using Bookshelf.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class ReviewsController(IReviewService reviewService) : Controller
{
    private const string ReviewsAnchor = "reviews";

    public async Task<IActionResult> Index(int? bookId, CancellationToken cancellationToken)
    {
        return View(await reviewService.GetAllAsync(bookId, cancellationToken));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    public IActionResult Create(int bookId)
    {
        return View(new ReviewFormViewModel { BookId = bookId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await reviewService.CreateAsync(model.ToCreateDto(), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddErrors(result);
            return View(model);
        }

        this.SetSuccess("Відгук додано.");
        return RedirectToBook(model.BookId);
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(ReviewFormViewModel.FromDetails(result.Value!)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReviewFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await reviewService.UpdateAsync(model.ToUpdateDto(id), cancellationToken);
        switch (result.Status)
        {
            case ResultStatus.NotFound:
                return NotFound();
            case ResultStatus.ValidationError:
                ModelState.AddErrors(result);
                return View(model);
        }

        this.SetSuccess("Відгук оновлено.");
        return RedirectToBook(model.BookId);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int bookId, CancellationToken cancellationToken)
    {
        var result = await reviewService.DeleteAsync(id, cancellationToken);
        if (result.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        this.SetSuccess("Відгук видалено.");
        return RedirectToBook(bookId);
    }

    private RedirectToActionResult RedirectToBook(int bookId) =>
        RedirectToAction(nameof(BooksController.Details), "Books", new { id = bookId }, ReviewsAnchor);
}
