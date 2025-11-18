using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByUserNameAsync(string username);

    // NEW — cần cho TrainerService
    Task<User?> GetByIdAsync(Guid id);

    Task<bool> CheckPasswordAsync(User user, string password);
    Task<(bool Succeeded, string? Error)> CreateAsync(User user, string password);

    Task EnsureRoleExistsAsync(string roleName);
    Task AddToRoleAsync(User user, string roleName);
    Task<IList<string>> GetRolesAsync(User user);
}