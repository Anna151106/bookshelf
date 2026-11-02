using System.ComponentModel.DataAnnotations;
using Bookshelf.Web.Controllers.Api;
using FluentAssertions;
using Xunit;

namespace Bookshelf.UnitTests.Controllers.Api;

public class PageQueryTests
{
    [Fact]
    public void Defaults_AreValid()
    {
        var query = new PageQuery();

        Validate(query).Should().BeEmpty();
        query.Skip.Should().Be(0);
        query.Limit.Should().Be(10);
    }

    [Theory]
    [InlineData(-1, 10, nameof(PageQuery.Skip))]
    [InlineData(0, 0, nameof(PageQuery.Limit))]
    [InlineData(0, 101, nameof(PageQuery.Limit))]
    public void InvalidValues_FailValidation(int skip, int limit, string member)
    {
        var errors = Validate(new PageQuery { Skip = skip, Limit = limit });

        errors.Should().ContainSingle().Which.MemberNames.Should().Equal(member);
    }

    private static List<ValidationResult> Validate(PageQuery query)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(query, new ValidationContext(query), results, validateAllProperties: true);
        return results;
    }
}
