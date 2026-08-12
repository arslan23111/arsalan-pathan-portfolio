using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstractions;

public interface IContactMessageRepository
{
    Task<ContactMessage> CreateAsync(ContactMessage message, CancellationToken cancellationToken);
    Task<IReadOnlyList<ContactMessage>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> SetReadStatusAsync(Guid id, bool isRead, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
