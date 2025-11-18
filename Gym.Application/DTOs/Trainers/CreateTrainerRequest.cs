namespace Gym.Application.DTOs.Trainers;

public class CreateTrainerRequest
{
    public required Guid UserId { get; set; }
    public string Specialty { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string Phone { get; set; } = "";
}