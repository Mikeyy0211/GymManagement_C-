using Gym.Core.Entities;

namespace Gym.Application.Auth;

public interface IJwtTokenGenerator
{
    Task<string> GenerateAsync(User user, IEnumerable<string> roles);
}