using Bookshelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Author> Authors { get; }

    DbSet<Genre> Genres { get; }

    DbSet<Book> Books { get; }

    DbSet<Review> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
