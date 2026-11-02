using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshelf.Infrastructure.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).IsRequired().HasMaxLength(Author.FullNameMaxLength);
        builder.Property(a => a.Biography).HasMaxLength(Author.BiographyMaxLength);
        builder.Property(a => a.Country).HasMaxLength(Author.CountryMaxLength);

        builder.HasData(SeedData.Authors);
    }
}
