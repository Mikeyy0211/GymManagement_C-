using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var users = sp.GetRequiredService<IUserRepository>();

        await users.EnsureRoleExistsAsync("Admin");
        await users.EnsureRoleExistsAsync("Trainer");
        await users.EnsureRoleExistsAsync("Member");

        var admin = await users.FindByUserNameAsync("admin");

        if (admin == null)
        {
            var newAdmin = new User
            {
                UserName = "admin",
                FullName = "Administrator"
            };

            var (ok, err) = await users.CreateAsync(newAdmin, "Admin123!");
            if (ok)
                await users.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}