using Bookshelf.Application.Books;
using Bookshelf.Application.Reviews;
using Bookshelf.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Bookshelf.Web.Controllers;

[Authorize]
[FeatureGate("PremiumPicks")]
public class PremiumController(IBookService bookService, IReviewService reviewService) : Controller
{
    public async Task<IActionResult> Picks(CancellationToken cancellationToken)
    {
        var books = await bookService.GetAllAsync(cancellationToken: cancellationToken);
        var reviews = await reviewService.GetAllAsync(cancellationToken: cancellationToken);

        var picks = reviews
            .GroupBy(r => r.BookId)
            .Select(g => new { BookId = g.Key, Average = g.Average(r => r.Rating), Count = g.Count() })
            .Join(books, r => r.BookId, b => b.Id, (r, b) => new BookPickViewModel(b, r.Average, r.Count))
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ReviewCount)
            .ThenBy(p => p.Book.Title)
            .Take(5)
            .ToList();

        return View(picks);
    }
}
