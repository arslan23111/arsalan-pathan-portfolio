using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Application.Authentication;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AdminCredentialValidator validator, AdminTokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        if (!validator.IsValid(request.Email, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var email = request.Email.Trim().ToLowerInvariant();
        return Ok(new { message = "Login successful.", email, token = tokens.Create(email) });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { email = User.Identity?.Name, role = "Admin" });

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout() => Ok(new { message = "Logout successful." });
}
