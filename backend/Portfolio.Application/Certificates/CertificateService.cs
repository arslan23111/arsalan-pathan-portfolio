using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Certificates;

public sealed class CertificateService(ICertificateRepository repository)
{
    public Task<IReadOnlyList<Certificate>> GetAllAsync(CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);

    public Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<Certificate> CreateAsync(CertificateRequest request, CancellationToken cancellationToken) =>
        repository.CreateAsync(Map(request), cancellationToken);

    public async Task<bool> UpdateAsync(Guid id, CertificateRequest request, CancellationToken cancellationToken)
    {
        var certificate = Map(request);
        certificate.Id = id;
        return await repository.UpdateAsync(certificate, cancellationToken);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        repository.DeleteAsync(id, cancellationToken);

    private static Certificate Map(CertificateRequest request) => new()
    {
        Title = request.Title.Trim(),
        Issuer = request.Issuer.Trim(),
        IssueYear = request.IssueYear,
        Description = request.Description.Trim(),
        FileUrl = Clean(request.FileUrl),
        FileType = Clean(request.FileType)
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
