using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByUserNameAsync(string username);

    // Required by UoW pattern
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByIdAsync(Guid id, bool asNoTracking, bool includeDeleted, CancellationToken ct);

    Task<bool> CheckPasswordAsync(User user, string password);
    Task<(bool Succeeded, string? Error)> CreateAsync(User user, string password);

    Task EnsureRoleExistsAsync(string roleName);
    Task AddToRoleAsync(User user, string roleName);
    Task<IList<string>> GetRolesAsync(User user);
}