using Bookshelf.Application.Books;

namespace Bookshelf.Web.ViewModels;

public class BookDetailsViewModel
{
    public required BookDetailsDto Book { get; init; }

    public required ReviewFormViewModel NewReview { get; init; }
}
