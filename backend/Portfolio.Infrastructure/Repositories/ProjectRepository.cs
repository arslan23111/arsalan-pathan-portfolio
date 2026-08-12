using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Abstractions;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories;

public sealed class ProjectRepository(PortfolioDbContext dbContext) : IProjectRepository
{
    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Projects.AsNoTracking().OrderByDescending(item => item.CreatedAt).ToListAsync(cancellationToken);

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Projects.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken)
    {
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<bool> UpdateAsync(Project project, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Projects.FindAsync([project.Id], cancellationToken);
        if (existing is null) return false;

        existing.Title = project.Title;
        existing.Description = project.Description;
        existing.ImageUrl = project.ImageUrl;
        existing.Technologies = project.Technologies;
        existing.Features = project.Features;
        existing.GitHubUrl = project.GitHubUrl;
        existing.LiveDemoUrl = project.LiveDemoUrl;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FindAsync([id], cancellationToken);
        if (project is null) return false;
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
