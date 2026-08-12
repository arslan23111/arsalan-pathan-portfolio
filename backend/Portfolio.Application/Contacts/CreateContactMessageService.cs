using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Contacts;

public sealed class CreateContactMessageService(IContactMessageRepository repository)
{
    public Task<ContactMessage> ExecuteAsync(
        CreateContactMessageRequest request,
        CancellationToken cancellationToken)
    {
        var message = new ContactMessage
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim()
        };

        return repository.CreateAsync(message, cancellationToken);
    }
}
