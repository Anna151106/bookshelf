using Bookshelf.Application.Common;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Common;

public class IsbnFormatTests
{
    [Theory]
    [InlineData("9780306406157", true)]
    [InlineData("978-0-306-40615-7", true)]
    [InlineData("080442957X", true)]
    [InlineData("9780306406158", false)]
    [InlineData("12345", false)]
    [InlineData("", false)]
    public void IsValid(string isbn, bool expected)
    {
        IsbnFormat.IsValid(isbn).Should().Be(expected);
    }
}
