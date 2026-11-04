using Bookshelf.Application.Books;
using Bookshelf.Application.Common;
using Bookshelf.Application.Reviews;
using Bookshelf.Web.Infrastructure;
using Bookshelf.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

[Authorize]
public class ReviewsController(IReviewService reviewService, IBookService bookService) : Controller
{
    private const string ReviewsAnchor = "reviews";

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? bookId, CancellationToken cancellationToken)
    {
        return View(await reviewService.GetAllAsync(bookId, cancellationToken));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    public async Task<IActionResult> Create(int bookId, CancellationToken cancellationToken)
    {
        return await FormViewAsync(new ReviewFormViewModel { BookId = bookId, CanChooseBook = true }, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewFormViewModel model, CancellationToken cancellationToken)
    {
        model.CanChooseBook = true;
        if (!ModelState.IsValid)
        {
            return await FormViewAsync(model, cancellationToken);
        }

        var result = await reviewService.CreateAsync(model.ToCreateDto(), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddErrors(result);
            return await FormViewAsync(model, cancellationToken);
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
            return await FormViewAsync(model, cancellationToken);
        }

        var result = await reviewService.UpdateAsync(model.ToUpdateDto(id), cancellationToken);
        switch (result.Status)
        {
            case ResultStatus.NotFound:
                return NotFound();
            case ResultStatus.ValidationError:
                ModelState.AddErrors(result);
                return await FormViewAsync(model, cancellationToken);
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

    private async Task<ViewResult> FormViewAsync(ReviewFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.BookId > 0)
        {
            model.BookTitle = (await bookService.GetByIdAsync(model.BookId, cancellationToken)).Value?.Title;
        }

        return View(model);
    }

        private RedirectToActionResult RedirectToBook(int bookId) =>
        RedirectToAction(nameof(BooksController.Details), "Books", new { id = bookId }, ReviewsAnchor);
}
