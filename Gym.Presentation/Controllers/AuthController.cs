using System.Security.Claims;
using Gym.Application.Auth;
using Gym.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var msg = await _auth.RegisterAsync(req);
        return Ok(new { message = msg });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
        => Ok(await _auth.LoginAsync(req));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return Ok(await _auth.MeAsync(username!));
    }
}