namespace Bookshelf.Application.Users;

public record AppUserDto(int Id, string Email, string Name, DateTime CreatedAt, string Plan);
