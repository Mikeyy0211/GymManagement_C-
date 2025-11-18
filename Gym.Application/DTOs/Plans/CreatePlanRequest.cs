namespace Gym.Application.DTOs.Plans;

public class CreatePlanRequest
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public int MaxSessionsPerWeek { get; set; }
}