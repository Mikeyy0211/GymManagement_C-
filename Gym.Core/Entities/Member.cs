namespace Gym.Core.Entities;
    
public class Member : BaseEntity
{
    public required string FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public Guid? MembershipPlanId { get; set; }
    public MembershipPlan? MembershipPlan { get; set; }

}

