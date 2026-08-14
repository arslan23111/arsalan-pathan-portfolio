using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Certificates;

public sealed class CertificateRequest
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string Title { get; init; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 2)]
    public string Issuer { get; init; } = string.Empty;

    [Range(2000, 2100)]
    public int IssueYear { get; init; }

    [StringLength(1000)]
    public string Description { get; init; } = string.Empty;

    [Url, StringLength(500)]
    public string? FileUrl { get; init; }

    [StringLength(20)]
    public string? FileType { get; init; }
}
