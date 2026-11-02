namespace Bookshelf.UnitTests.Infrastructure;

public sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public static readonly FixedTimeProvider Default = new(new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero));

    public override DateTimeOffset GetUtcNow() => now;
}
