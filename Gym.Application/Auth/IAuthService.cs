using Gym.Application.DTOs.Auth;

namespace Gym.Application.Auth;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<MeResponse> MeAsync(string username);
}