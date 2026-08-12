using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Projects;

public sealed class ProjectRequest
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string Title { get; init; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Description { get; init; } = string.Empty;

    [Url, StringLength(500)]
    public string? ImageUrl { get; init; }

    [Required, StringLength(500)]
    public string Technologies { get; init; } = string.Empty;

    [StringLength(1500)]
    public string Features { get; init; } = string.Empty;

    [Url, StringLength(500)]
    public string? GitHubUrl { get; init; }

    [Url, StringLength(500)]
    public string? LiveDemoUrl { get; init; }
}
