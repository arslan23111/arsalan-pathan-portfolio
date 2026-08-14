namespace Portfolio.Domain.Entities;

public sealed class Certificate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int IssueYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
