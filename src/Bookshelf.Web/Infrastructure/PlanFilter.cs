using Bookshelf.Domain.Entities;
using Microsoft.FeatureManagement;

namespace Bookshelf.Web.Infrastructure;

[FilterAlias("Plan")]
public class PlanFilter(IHttpContextAccessor httpContextAccessor) : IFeatureFilter
{
    public const string ClaimType = "plan";

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var plan = httpContextAccessor.HttpContext?.User.FindFirst(ClaimType)?.Value;
        return Task.FromResult(plan == AppUser.PremiumPlan);
    }
}
