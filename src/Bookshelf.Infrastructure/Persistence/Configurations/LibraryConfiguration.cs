using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshelf.Infrastructure.Persistence.Configurations;

public class LibraryConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).IsRequired().HasMaxLength(Library.NameMaxLength);
        builder.Property(l => l.Address).IsRequired().HasMaxLength(Library.AddressMaxLength);
        builder.Property(l => l.District).IsRequired().HasMaxLength(Library.DistrictMaxLength);
        builder.HasIndex(l => l.District);

        builder.HasMany(l => l.Books)
            .WithMany(b => b.Libraries)
            .UsingEntity<Dictionary<string, object>>(
                "LibraryBooks",
                j => j.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Library>().WithMany().HasForeignKey("LibraryId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("LibraryId", "BookId"));

        builder.HasData(SeedData.Libraries);
    }
}
