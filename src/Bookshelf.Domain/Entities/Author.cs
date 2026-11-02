namespace Bookshelf.Domain.Entities;

public class Author
{
    public const int FullNameMaxLength = 200;
    public const int BiographyMaxLength = 4000;
    public const int CountryMaxLength = 100;

    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Biography { get; set; }

    public int? BirthYear { get; set; }

    public string? Country { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
