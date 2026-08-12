using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories;

public sealed class ContactMessageRepository(PortfolioDbContext dbContext) : IContactMessageRepository
{
    public async Task<ContactMessage> CreateAsync(ContactMessage message, CancellationToken cancellationToken)
    {
        dbContext.ContactMessages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task<IReadOnlyList<ContactMessage>> GetAllAsync(CancellationToken token) =>
        await dbContext.ContactMessages.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync(token);

    public async Task<bool> SetReadStatusAsync(Guid id, bool isRead, CancellationToken token)
    {
        var item = await dbContext.ContactMessages.FindAsync([id], token);
        if (item is null) return false;
        item.IsRead = isRead;
        await dbContext.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token)
    {
        var item = await dbContext.ContactMessages.FindAsync([id], token);
        if (item is null) return false;
        dbContext.ContactMessages.Remove(item);
        await dbContext.SaveChangesAsync(token);
        return true;
    }
}
using Microsoft.EntityFrameworkCore;
