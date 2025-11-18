namespace Gym.Application.DTOs.Members;

public class UpdateMemberRequest
{
    public string FullName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public Guid? MembershipPlanId { get; set; } 

    public string? RowVersionBase64 { get; set; } 
}