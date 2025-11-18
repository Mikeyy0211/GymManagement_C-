namespace Gym.Application.DTOs.Auth;

public class MeResponse
{
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public IEnumerable<string> Roles { get; set; } = default!;
}