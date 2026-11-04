using Bookshelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshelf.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(AppUser.EmailMaxLength);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(AppUser.NameMaxLength);
        builder.Property(u => u.Plan).IsRequired().HasMaxLength(AppUser.PlanMaxLength).HasDefaultValue(AppUser.FreePlan);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
