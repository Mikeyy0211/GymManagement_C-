namespace Gym.Application.DTOs.Classes;

public class CreateSessionRequest
{
    public Guid ClassId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int? CapacityOverride { get; set; }
}