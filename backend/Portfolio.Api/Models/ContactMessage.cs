namespace Portfolio.Api.Models;

public sealed class ContactMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public required string Subject { get; init; }
    public required string Message { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsRead { get; set; }
}
