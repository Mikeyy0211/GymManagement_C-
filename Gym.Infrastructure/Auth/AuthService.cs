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
    private readonly IUnitOfWork _uow;

    public AuthService(
        IUserRepository users,
        IJwtTokenGenerator jwt,
        IMemberRepository members,
        ITrainerRepository trainers,
        IUnitOfWork uow)
    {
        _users = users;
        _jwt = jwt;
        _members = members;
        _trainers = trainers;
        _uow = uow;
    }

    // ======================================================
    // REGISTER
    // ======================================================
    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        // đảm bảo role tồn tại
        await _users.EnsureRoleExistsAsync(request.Role);

        // tạo user
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username!,
            FullName = request.FullName!,
            DateOfBirth = request.DateOfBirth,
            Email = $"{request.Username!}@example.com"
        };

        // tạo user trong Identity
        var (ok, error) = await _users.CreateAsync(user, request.Password);
        if (!ok)
            throw new InvalidOperationException(error ?? "Failed to create user");

        // gán role
        await _users.AddToRoleAsync(user, request.Role);

        // ======================================================
        // Tạo thêm thực thể Member / Trainer
        // ======================================================

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

    // ======================================================
    // LOGIN
    // ======================================================
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.FindByUserNameAsync(request.Username!)
                   ?? throw new UnauthorizedAccessException("Invalid username or password");

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
            FullName = user.FullName ?? ""
        };
    }

    // ======================================================
    // ME()
    // ======================================================
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