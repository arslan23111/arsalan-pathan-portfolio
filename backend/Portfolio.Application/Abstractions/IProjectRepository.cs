using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstractions;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Project> CreateAsync(Project project, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Project project, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
