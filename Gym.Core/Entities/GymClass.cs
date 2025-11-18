namespace Gym.Core.Entities;

public class GymClass : BaseEntity
{
    public required string Name { get; set; }
    public Guid TrainerId { get; set; }
    public TrainerProfile Trainer { get; set; } = default!;
    public int Capacity { get; set; }

    public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
}