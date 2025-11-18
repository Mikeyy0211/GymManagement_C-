using System.ComponentModel.DataAnnotations.Schema;

namespace Gym.Core.Entities;

public class MembershipPlan : BaseEntity
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public int MaxSessionsPerWeek { get; set; }

}