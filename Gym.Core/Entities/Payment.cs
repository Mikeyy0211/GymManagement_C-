namespace Gym.Core.Entities;

public class Payment : BaseEntity
{
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = default!;
    
    public Guid PlanId { get; set; }
    public MembershipPlan Plan { get; set; } = default!;
    
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpireAt { get; set; }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Success;

}