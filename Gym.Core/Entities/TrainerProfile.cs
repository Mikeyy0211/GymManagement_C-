namespace Gym.Core.Entities;

public class TrainerProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public string Specialty { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string Phone { get; set; } = "";
}