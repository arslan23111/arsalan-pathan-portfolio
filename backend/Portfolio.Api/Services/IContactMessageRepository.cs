using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public interface IContactMessageRepository
{
    Task<ContactMessage> CreateAsync(ContactMessage message, CancellationToken cancellationToken);
}
