using System.Security.Claims;
using Bookshelf.Web.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Xunit;

namespace Bookshelf.UnitTests.Infrastructure;

public class PlanFilterTests
{
    [Theory]
    [InlineData("Premium", true)]
    [InlineData("Free", false)]
    public async Task Evaluate_EnablesOnlyForPremium(string plan, bool expected)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(PlanFilter.ClaimType, plan)], "Test")),
        };
        var filter = new PlanFilter(new HttpContextAccessor { HttpContext = httpContext });

        var enabled = await filter.EvaluateAsync(new FeatureFilterEvaluationContext { FeatureName = "PremiumPicks" });

        enabled.Should().Be(expected);
    }
}
