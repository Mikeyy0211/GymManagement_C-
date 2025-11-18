using Gym.Application.Auth;
using Gym.Application.DTOs.Auth;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IMemberRepository _members;
    private readonly ITrainerRepository _trainers;

    public AuthService(
        IUserRepository users,
        IJwtTokenGenerator jwt,
        IMemberRepository members,
        ITrainerRepository trainers)
    {
        _users = users;
        _jwt = jwt;
        _members = members;
        _trainers = trainers;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        await _users.EnsureRoleExistsAsync(request.Role);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Email = $"{request.Username}@example.com"
        };

        var (ok, error) = await _users.CreateAsync(user, request.Password);
        if (!ok)
            throw new InvalidOperationException(error ?? "Failed to create user");

        await _users.AddToRoleAsync(user, request.Role);

        if (request.Role == "Member")
        {
            var m = new Member
            {
                FullName = request.FullName!,
                DateOfBirth = request.DateOfBirth
            };

            await _members.AddAsync(m, CancellationToken.None);
        }

        else if (request.Role == "Trainer")
        {
            var t = new TrainerProfile
            {
                UserId = user.Id,
                Specialty = request.Specialty ?? "General",
                ExperienceYears = request.ExperienceYears ?? 0,
                Phone = request.Phone ?? ""
            };

            await _trainers.AddAsync(t, CancellationToken.None);
        }

        return "Register successfully";
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.FindByUserNameAsync(request.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        var valid = await _users.CheckPasswordAsync(user, request.Password);
        if (!valid)
            throw new UnauthorizedAccessException("Invalid username or password");

        var roles = await _users.GetRolesAsync(user);
        var token = await _jwt.GenerateAsync(user, roles);

        return new AuthResponse
        {
            Token = token,
            Roles = roles,
            Username = user.UserName!,
            FullName = user.FullName
        };
    }

    public async Task<MeResponse> MeAsync(string username)
    {
        var user = await _users.FindByUserNameAsync(username)
                   ?? throw new KeyNotFoundException("User not found");

        var roles = await _users.GetRolesAsync(user);

        return new MeResponse
        {
            Username = user.UserName!,
            FullName = user.FullName ?? "",
            Roles = roles
        };
    }
}
