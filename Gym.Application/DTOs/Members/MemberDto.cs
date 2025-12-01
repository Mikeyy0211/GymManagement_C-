namespace Gym.Application.DTOs.Members;

public class MemberDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public Guid? MembershipPlanId { get; set; }
    public string? MembershipPlanName { get; set; }

    public string RowVersionBase64 { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}