namespace Gym.Application.DTOs.Plans;

public class PlanQuery
{
    public string? Search { get; set; }        
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "CreatedAt"; // Name|Price|CreatedAt
    public string? SortDir { get; set; } = "desc";     // asc|desc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

}