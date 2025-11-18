using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gym.Application.Auth;
using Gym.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gym.Infrastructure.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;
    public JwtTokenGenerator(IConfiguration config) => _config = config;

    public Task<string> GenerateAsync(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new("fullName", user.FullName ?? string.Empty),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds
        );
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
