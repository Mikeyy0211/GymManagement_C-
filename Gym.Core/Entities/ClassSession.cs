namespace Gym.Core.Entities;

public class ClassSession : BaseEntity
{
    public Guid GymClassId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? CapacityOverride { get; set; }
    
    public GymClass? GymClass { get; set; }

}