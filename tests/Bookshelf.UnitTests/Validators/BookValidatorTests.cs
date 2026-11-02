using Bookshelf.Application.Books;
using Bookshelf.UnitTests.Infrastructure;
using FluentValidation.TestHelper;
using Xunit;

namespace Bookshelf.UnitTests.Validators;

public sealed class BookValidatorTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly CreateBookDtoValidator validator;
    private readonly CreateBookDto valid;

    public BookValidatorTests()
    {
        validator = new CreateBookDtoValidator(database.Context, FixedTimeProvider.Default);

        valid = new CreateBookDto
        {
            Title = "Animal Farm",
            PublishedYear = 1945,
            Isbn = "9780306406157",
            Pages = 112,
            AuthorId = database.AddAuthor().Id,
            GenreId = database.AddGenre().Id,
        };
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task ValidBook_HasNoErrors()
    {
        var result = await validator.TestValidateAsync(valid, cancellationToken: Ct);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(2026, 112, "9780306406157", nameof(CreateBookDto.PublishedYear))]
    [InlineData(1945, 0, "9780306406157", nameof(CreateBookDto.Pages))]
    [InlineData(1945, 112, "12345", nameof(CreateBookDto.Isbn))]
    [InlineData(1945, 112, "9780306406158", nameof(CreateBookDto.Isbn))]
    public async Task InvalidField_HasError(int year, int pages, string isbn, string property)
    {
        var book = valid with { PublishedYear = year, Pages = pages, Isbn = isbn };

        var result = await validator.TestValidateAsync(book, cancellationToken: Ct);

        result.ShouldHaveValidationErrorFor(property);
    }

    [Fact]
    public async Task DuplicateIsbn_HasError()
    {
        database.AddBook(database.AddAuthor("Other"), database.AddGenre("Other"), isbn: "9780306406157");

        var result = await validator.TestValidateAsync(valid, cancellationToken: Ct);

        result.ShouldHaveValidationErrorFor(x => x.Isbn).WithErrorMessage("Книга з таким ISBN уже є.");
    }
}
