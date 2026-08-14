using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence;

public sealed class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Certificate> Certificates => Set<Certificate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var contact = modelBuilder.Entity<ContactMessage>();
        contact.ToTable("ContactMessages");
        contact.HasKey(item => item.Id);
        contact.Property(item => item.Name).HasMaxLength(100).IsRequired();
        contact.Property(item => item.Email).HasMaxLength(200).IsRequired();
        contact.Property(item => item.Phone).HasMaxLength(30);
        contact.Property(item => item.Subject).HasMaxLength(150).IsRequired();
        contact.Property(item => item.Message).HasMaxLength(3000).IsRequired();

        var project = modelBuilder.Entity<Project>();
        project.ToTable("Projects");
        project.HasKey(item => item.Id);
        project.Property(item => item.Title).HasMaxLength(150).IsRequired();
        project.Property(item => item.Description).HasMaxLength(2000).IsRequired();
        project.Property(item => item.ImageUrl).HasMaxLength(500);
        project.Property(item => item.Technologies).HasMaxLength(500).IsRequired();
        project.Property(item => item.Features).HasMaxLength(1500);
        project.Property(item => item.GitHubUrl).HasMaxLength(500);
        project.Property(item => item.LiveDemoUrl).HasMaxLength(500);

        var certificate = modelBuilder.Entity<Certificate>();
        certificate.ToTable("Certificates");
        certificate.HasKey(item => item.Id);
        certificate.Property(item => item.Title).HasMaxLength(150).IsRequired();
        certificate.Property(item => item.Issuer).HasMaxLength(150).IsRequired();
        certificate.Property(item => item.Description).HasMaxLength(1000);
        certificate.Property(item => item.FileUrl).HasMaxLength(500);
        certificate.Property(item => item.FileType).HasMaxLength(20);
    }
}
