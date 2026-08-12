using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Projects;

public sealed class ProjectService(IProjectRepository repository)
{
    public Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<Project> CreateAsync(ProjectRequest request, CancellationToken cancellationToken) =>
        repository.CreateAsync(Map(request), cancellationToken);

    public async Task<bool> UpdateAsync(Guid id, ProjectRequest request, CancellationToken cancellationToken)
    {
        var project = Map(request);
        project.Id = id;
        return await repository.UpdateAsync(project, cancellationToken);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        repository.DeleteAsync(id, cancellationToken);

    private static Project Map(ProjectRequest request) => new()
    {
        Title = request.Title.Trim(),
        Description = request.Description.Trim(),
        ImageUrl = Clean(request.ImageUrl),
        Technologies = request.Technologies.Trim(),
        Features = request.Features.Trim(),
        GitHubUrl = Clean(request.GitHubUrl),
        LiveDemoUrl = Clean(request.LiveDemoUrl)
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
