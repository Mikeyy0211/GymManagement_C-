namespace Gym.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public IEnumerable<string> Roles { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string? FullName { get; set; }
}