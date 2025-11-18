namespace Gym.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!; // Admin / Trainer / Member

    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Trainer options
    public string? Specialty { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Phone { get; set; }
}