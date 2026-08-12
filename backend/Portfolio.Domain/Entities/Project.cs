namespace Portfolio.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Technologies { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public string? GitHubUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
