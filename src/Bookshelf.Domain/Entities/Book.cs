namespace Bookshelf.Domain.Entities;

public class Book
{
    public const int TitleMaxLength = 300;
    public const int DescriptionMaxLength = 4000;
    public const int IsbnMaxLength = 20;
    public const int CoverPathMaxLength = 500;

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int PublishedYear { get; set; }

    public string Isbn { get; set; } = string.Empty;

    public int Pages { get; set; }

    public string? CoverPath { get; set; }

    public int AuthorId { get; set; }

    public int GenreId { get; set; }

    public Author Author { get; set; } = null!;

    public Genre Genre { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
