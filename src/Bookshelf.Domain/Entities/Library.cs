namespace Bookshelf.Domain.Entities;

public class Library
{
    public const int NameMaxLength = 200;
    public const int AddressMaxLength = 300;
    public const int DistrictMaxLength = 100;

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
