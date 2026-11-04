using Bookshelf.Application.Users;
using Bookshelf.UnitTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookshelf.UnitTests.Services;

public sealed class UserServiceTests : IDisposable
{
    private readonly TestDatabase database = new();
    private readonly UserService service;

    public UserServiceTests()
    {
        service = new UserService(database.Context, FixedTimeProvider.Default);
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose() => database.Dispose();

    [Fact]
    public async Task FirstLogin_CreatesUser()
    {
        var user = await service.FindOrCreateAsync("olena@example.com", "Olena", Ct);

        user.Email.Should().Be("olena@example.com");
        user.Name.Should().Be("Olena");
        user.CreatedAt.Should().Be(FixedTimeProvider.Default.GetUtcNow().UtcDateTime);
        (await database.CreateContext().Users.CountAsync(Ct)).Should().Be(1);
    }

    [Fact]
    public async Task RepeatedLogin_ReturnsSameUserWithoutDuplicate()
    {
        var first = await service.FindOrCreateAsync("olena@example.com", "Olena", Ct);

        var second = await service.FindOrCreateAsync("Olena@Example.com", "Olena K.", Ct);

        second.Id.Should().Be(first.Id);
        (await database.CreateContext().Users.CountAsync(Ct)).Should().Be(1);
    }

    [Fact]
    public async Task ChangePlan_SwitchesToPremiumAndBack()
    {
        var created = await service.FindOrCreateAsync("olena@example.com", "Olena", Ct);
        created.Plan.Should().Be("Free");

        var premium = await service.ChangePlanAsync("olena@example.com", "Premium", Ct);
        premium!.Plan.Should().Be("Premium");
        (await service.GetByEmailAsync("olena@example.com", Ct))!.Plan.Should().Be("Premium");

        var free = await service.ChangePlanAsync("olena@example.com", "Free", Ct);
        free!.Plan.Should().Be("Free");
    }
}
