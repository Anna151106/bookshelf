using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshelf.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint("CK_Books_Pages", "\"Pages\" > 0"));

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(Book.TitleMaxLength);
        builder.Property(b => b.Description).HasMaxLength(Book.DescriptionMaxLength);
        builder.Property(b => b.Isbn).IsRequired().HasMaxLength(Book.IsbnMaxLength);
        builder.Property(b => b.CoverPath).HasMaxLength(Book.CoverPathMaxLength);
        builder.HasIndex(b => b.Isbn).IsUnique();

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.Books);
    }
}
