namespace Bookshelf.Domain.Entities;

public class Review
{
    public const int ReaderNameMaxLength = 100;
    public const int TextMaxLength = 2000;
    public const int MinRating = 1;
    public const int MaxRating = 5;

    public int Id { get; set; }

    public int BookId { get; set; }

    public string ReaderName { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; }

    public Book Book { get; set; } = null!;
}
