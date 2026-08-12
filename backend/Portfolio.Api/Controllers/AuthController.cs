using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Application.Authentication;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AdminCredentialValidator validator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!validator.IsValid(request.Email, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, request.Email.Trim().ToLowerInvariant()), new Claim(ClaimTypes.Role, "Admin")],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2) });

        return Ok(new { message = "Login successful.", email = identity.Name });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { email = User.Identity?.Name, role = "Admin" });

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logout successful." });
    }
}
