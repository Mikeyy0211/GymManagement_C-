using System.Security.Claims;
using Gym.Application.Auth;
using Gym.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

    [SwaggerOperation(Summary = "Register a new user", Description = "Allows Admin/Trainer/Member to register.")]
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var msg = await _auth.RegisterAsync(req);
        return Ok(new { message = msg });
    }

    [SwaggerOperation(Summary = "Login", Description = "Returns JWT token for authentication.")]
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
        => Ok(await _auth.LoginAsync(req));

    [SwaggerOperation(Summary = "Get current user info", Description = "Reads JWT token and returns user profile.")]
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return Ok(await _auth.MeAsync(username!));
    }
}