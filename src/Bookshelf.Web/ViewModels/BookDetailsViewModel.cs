using Bookshelf.Application.Authors;
using Bookshelf.Application.Books;
using Bookshelf.Web.Infrastructure;

namespace Bookshelf.Web.ViewModels;

public class BookDetailsViewModel
{
    public required BookDetailsDto Book { get; init; }

    public AuthorDetailsDto? Author { get; init; }

    public required LibraryContactsOptions Contacts { get; init; }

    public required ReviewFormViewModel NewReview { get; init; }
}
