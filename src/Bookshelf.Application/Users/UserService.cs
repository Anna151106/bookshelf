using Bookshelf.Application.Abstractions;
using Bookshelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Application.Users;

public class UserService(IAppDbContext db, TimeProvider timeProvider) : IUserService
{
    public async Task<AppUserDto> FindOrCreateAsync(string email, string name, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);

        if (user is null)
        {
            user = new AppUser
            {
                Email = normalized,
                Name = string.IsNullOrWhiteSpace(name) ? normalized : name.Trim(),
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);
        }

        return ToDto(user);
    }

    public async Task<AppUserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
        return user is null ? null : ToDto(user);
    }

    public async Task<AppUserDto?> ChangePlanAsync(string email, string plan, CancellationToken cancellationToken = default)
    {
        if (plan is not (AppUser.FreePlan or AppUser.PremiumPlan))
        {
            throw new ArgumentException($"Unknown plan '{plan}'.", nameof(plan));
        }

        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.Plan = plan;
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(user);
    }

    private static AppUserDto ToDto(AppUser user) => new(user.Id, user.Email, user.Name, user.CreatedAt, user.Plan);
}
