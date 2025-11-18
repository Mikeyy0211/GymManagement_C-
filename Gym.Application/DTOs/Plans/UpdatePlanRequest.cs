namespace Gym.Application.DTOs.Plans;

public class UpdatePlanRequest
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public int MaxSessionsPerWeek { get; set; }

    public string? RowVersionBase64 { get; set; }
}