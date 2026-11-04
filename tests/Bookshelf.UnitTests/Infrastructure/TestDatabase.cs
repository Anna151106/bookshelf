using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

        Context.Libraries.ExecuteDelete();
        Context.Reviews.ExecuteDelete();
        Context.Books.ExecuteDelete();
        Context.Authors.ExecuteDelete();
        Context.Genres.ExecuteDelete();
    }

    public AppDbContext Context { get; }

    public AppDbContext CreateContext(params IInterceptor[] interceptors) =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).AddInterceptors(interceptors).Options);

    public Author AddAuthor(string fullName = "George Orwell") =>
        Add(new Author { FullName = fullName });

    public Genre AddGenre(string name = "Dystopia") =>
        Add(new Genre { Name = name });

    public Book AddBook(Author author, Genre genre, string title = "Animal Farm", string isbn = "9780000000019", string? coverPath = null, int publishedYear = 1945) =>
        Add(new Book
        {
            Title = title,
            Isbn = isbn,
            PublishedYear = publishedYear,
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

    public Library AddLibrary(string name, string district, params Book[] books)
    {
        using var context = CreateContext();
        var library = new Library { Name = name, Address = "вул. Тестова, 1", District = district, Latitude = 50.45, Longitude = 30.52 };
        foreach (var book in books)
        {
            var tracked = new Book { Id = book.Id };
            context.Books.Attach(tracked);
            library.Books.Add(tracked);
        }

        context.Libraries.Add(library);
        context.SaveChanges();
        return library;
    }

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
