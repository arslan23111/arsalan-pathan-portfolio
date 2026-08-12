using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Contacts;

public sealed class CreateContactMessageRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Phone { get; init; }

    [Required, StringLength(150, MinimumLength = 3)]
    public string Subject { get; init; } = string.Empty;

    [Required, StringLength(3000, MinimumLength = 10)]
    public string Message { get; init; } = string.Empty;
}
