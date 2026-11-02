using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.UnitTests.Infrastructure;

public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection connection;

    public TestDatabase()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        Context = CreateContext();
        Context.Database.EnsureCreated();

        Context.Reviews.ExecuteDelete();
        Context.Books.ExecuteDelete();
        Context.Authors.ExecuteDelete();
        Context.Genres.ExecuteDelete();
    }

    public AppDbContext Context { get; }

    public AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);

    public Author AddAuthor(string fullName = "George Orwell") =>
        Add(new Author { FullName = fullName });

    public Genre AddGenre(string name = "Dystopia") =>
        Add(new Genre { Name = name });

    public Book AddBook(Author author, Genre genre, string title = "Animal Farm", string isbn = "9780000000019", string? coverPath = null) =>
        Add(new Book
        {
            Title = title,
            Isbn = isbn,
            PublishedYear = 1945,
            Pages = 112,
            AuthorId = author.Id,
            GenreId = genre.Id,
            CoverPath = coverPath,
        });

    public Review AddReview(Book book, int rating = 5) =>
        Add(new Review
        {
            BookId = book.Id,
            ReaderName = "Reader",
            Rating = rating,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        });

    public void Dispose()
    {
        Context.Dispose();
        connection.Dispose();
    }

    private T Add<T>(T entity)
        where T : class
    {
        using var context = CreateContext();
        context.Add(entity);
        context.SaveChanges();
        return entity;
    }
}
