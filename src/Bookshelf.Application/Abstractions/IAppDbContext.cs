using Bookshelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Author> Authors { get; }

    DbSet<Genre> Genres { get; }

    DbSet<Book> Books { get; }

    DbSet<Review> Reviews { get; }

    DbSet<Library> Libraries { get; }

    DbSet<AppUser> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
