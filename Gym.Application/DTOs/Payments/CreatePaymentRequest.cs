namespace Gym.Application.DTOs.Payments;

public class CreatePaymentRequest
{
    public Guid MemberId { get; set; }
    public Guid PlanId { get; set; }
}