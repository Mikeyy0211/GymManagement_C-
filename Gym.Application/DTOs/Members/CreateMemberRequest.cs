namespace Gym.Application.DTOs.Members;

public class CreateMemberRequest
{
    public string FullName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public Guid? MembershipPlanId { get; set; } 
}