using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstractions;

public interface ICertificateRepository
{
    Task<IReadOnlyList<Certificate>> GetAllAsync(CancellationToken cancellationToken);
    Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Certificate> CreateAsync(Certificate certificate, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Certificate certificate, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
