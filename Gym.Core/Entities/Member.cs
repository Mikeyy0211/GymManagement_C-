using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Gym.Core.Entities;

namespace Gym.Core.Entities;
    
public class Member : BaseEntity
{
    public required string FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public Guid? MembershipPlanId { get; set; }
    public MembershipPlan? MembershipPlan { get; set; }
}

