namespace Gym.Application.DTOs.Members;

public class MemberQuery
{
    public string? Search { get; set; } // theo tên
    public Guid? PlanId { get; set; }
    public DateTime? DobFrom { get; set; }
    public DateTime? DobTo { get; set; }
    public string? SortBy { get; set; } = "CreatedAt"; // FullName|CreatedAt|DateOfBirth
    public string? SortDir { get; set; } = "desc";     // asc|desc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}