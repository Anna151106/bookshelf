using Bookshelf.Domain.Entities;
using Bookshelf.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshelf.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating", "\"Rating\" BETWEEN 1 AND 5"));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReaderName).IsRequired().HasMaxLength(Review.ReaderNameMaxLength);
        builder.Property(r => r.Text).HasMaxLength(Review.TextMaxLength);

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.Reviews);
    }
}
