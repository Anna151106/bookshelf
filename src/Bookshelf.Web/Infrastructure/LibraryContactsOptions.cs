namespace Bookshelf.Web.Infrastructure;

public class LibraryContactsOptions
{
    public const string SectionName = "LibraryContacts";

    public string Phone { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;
}
