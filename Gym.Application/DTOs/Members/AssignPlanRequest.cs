namespace Gym.Application.DTOs.Members;

public class AssignPlanRequest
{
    public Guid MembershipPlanId { get; set; }
    public string RowVersionBase64 { get; set; } = "";}