using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshelf.Infrastructure.Persistence.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).IsRequired().HasMaxLength(Genre.NameMaxLength);
        builder.HasIndex(g => g.Name).IsUnique();

        builder.HasData(SeedData.Genres);
    }
}
