using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Contacts;

public sealed class ContactMessageAdminService(IContactMessageRepository repository)
{
    public Task<IReadOnlyList<ContactMessage>> GetAllAsync(CancellationToken token) => repository.GetAllAsync(token);
    public Task<bool> SetReadStatusAsync(Guid id, bool isRead, CancellationToken token) => repository.SetReadStatusAsync(id, isRead, token);
    public Task<bool> DeleteAsync(Guid id, CancellationToken token) => repository.DeleteAsync(id, token);
}
