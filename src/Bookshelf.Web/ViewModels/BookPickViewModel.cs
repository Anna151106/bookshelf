using Bookshelf.Application.Books;

namespace Bookshelf.Web.ViewModels;

public record BookPickViewModel(BookListDto Book, double AverageRating, int ReviewCount);
