using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories;

public sealed class CertificateRepository(PortfolioDbContext dbContext) : ICertificateRepository
{
    public async Task<IReadOnlyList<Certificate>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Certificates.AsNoTracking().OrderByDescending(item => item.IssueYear).ThenByDescending(item => item.CreatedAt).ToListAsync(cancellationToken);

    public Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Certificates.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<Certificate> CreateAsync(Certificate certificate, CancellationToken cancellationToken)
    {
        dbContext.Certificates.Add(certificate);
        await dbContext.SaveChangesAsync(cancellationToken);
        return certificate;
    }

    public async Task<bool> UpdateAsync(Certificate certificate, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Certificates.FindAsync([certificate.Id], cancellationToken);
        if (existing is null) return false;
        existing.Title = certificate.Title;
        existing.Issuer = certificate.Issuer;
        existing.IssueYear = certificate.IssueYear;
        existing.Description = certificate.Description;
        existing.FileUrl = certificate.FileUrl;
        existing.FileType = certificate.FileType;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var certificate = await dbContext.Certificates.FindAsync([id], cancellationToken);
        if (certificate is null) return false;
        dbContext.Certificates.Remove(certificate);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
