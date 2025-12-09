namespace Gym.Application.DTOs.Classes;

public class SessionDto
{
    public Guid Id { get; set; }
    public Guid GymClassId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? CapacityOverride { get; set; }
}