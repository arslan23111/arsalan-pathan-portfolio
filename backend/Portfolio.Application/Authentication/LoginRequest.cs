using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Authentication;

public sealed class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
