namespace Bookshelf.Domain.Entities;

public class AppUser
{
    public const int EmailMaxLength = 256;
    public const int NameMaxLength = 200;
    public const int PlanMaxLength = 20;
    public const string FreePlan = "Free";
    public const string PremiumPlan = "Premium";

    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string Plan { get; set; } = FreePlan;
}
