namespace Gym.Application.DTOs.Trainers;

public class TrainerDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Specialty { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string Phone { get; set; } = "";
    
    public DateTime CreatedAt { get; set; }
}