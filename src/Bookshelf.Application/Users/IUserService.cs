namespace Bookshelf.Application.Users;

public interface IUserService
{
    Task<AppUserDto> FindOrCreateAsync(string email, string name, CancellationToken cancellationToken = default);

    Task<AppUserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<AppUserDto?> ChangePlanAsync(string email, string plan, CancellationToken cancellationToken = default);
}
