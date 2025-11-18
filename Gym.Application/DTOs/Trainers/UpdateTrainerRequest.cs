namespace Gym.Application.DTOs.Trainers;

public class UpdateTrainerRequest
{
    public string Specialty { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string Phone { get; set; } = "";
}