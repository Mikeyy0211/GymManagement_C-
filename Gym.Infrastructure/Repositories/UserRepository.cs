using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gym.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    
    public UserRepository(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public Task<User?> FindByUserNameAsync(string username)
        => _userManager.FindByNameAsync(username)!;

    public Task<User?> GetByIdAsync(Guid id)
        => _userManager.FindByIdAsync(id.ToString());

    public Task<bool> CheckPasswordAsync(User user, string password)
        => _userManager.CheckPasswordAsync(user, password);

    public async Task<(bool Succeeded, string? Error)> CreateAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    public async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    public Task AddToRoleAsync(User user, string roleName)
        => _userManager.AddToRoleAsync(user, roleName);

    public Task<IList<string>> GetRolesAsync(User user)
        => _userManager.GetRolesAsync(user);
}