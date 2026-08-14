using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Services;
using Portfolio.Application.Abstractions;
using Portfolio.Application.Contacts;
using Portfolio.Application.Certificates;
using Portfolio.Application.Projects;
using Portfolio.Infrastructure;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
}

var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(renderPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");
}

builder.Services.AddControllers();
builder.Services.AddScoped<AdminCredentialValidator>();
builder.Services.AddScoped<AdminTokenService>();
builder.Services.AddAuthentication(AdminTokenAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, AdminTokenAuthenticationHandler>(
        AdminTokenAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization();
builder.Services.AddScoped<CreateContactMessageService>();
builder.Services.AddScoped<ContactMessageAdminService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<CertificateService>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
var databaseConnection = builder.Configuration.GetConnectionString("PortfolioDatabase")
    ?? throw new InvalidOperationException("Portfolio database connection string is missing.");

builder.Services.AddPortfolioDatabase(databaseProvider, databaseConnection);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("PortfolioFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("ContactForm", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    database.Database.EnsureCreated();
    if (database.Database.IsSqlServer())
    {
        database.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'[dbo].[Projects]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[Projects] (
                    [Id] uniqueidentifier NOT NULL PRIMARY KEY,
                    [Title] nvarchar(150) NOT NULL,
                    [Description] nvarchar(2000) NOT NULL,
                    [ImageUrl] nvarchar(500) NULL,
                    [Technologies] nvarchar(500) NOT NULL,
                    [Features] nvarchar(1500) NOT NULL,
                    [GitHubUrl] nvarchar(500) NULL,
                    [LiveDemoUrl] nvarchar(500) NULL,
                    [CreatedAt] datetimeoffset NOT NULL
                );
            END
            """);
        database.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'[dbo].[Certificates]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[Certificates] (
                    [Id] uniqueidentifier NOT NULL PRIMARY KEY,
                    [Title] nvarchar(150) NOT NULL,
                    [Issuer] nvarchar(150) NOT NULL,
                    [IssueYear] int NOT NULL,
                    [Description] nvarchar(1000) NOT NULL,
                    [FileUrl] nvarchar(500) NULL,
                    [FileType] nvarchar(20) NULL,
                    [CreatedAt] datetimeoffset NOT NULL
                );
            END
            """);
    }
    else if (database.Database.IsNpgsql())
    {
        database.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "Certificates" (
                "Id" uuid NOT NULL PRIMARY KEY,
                "Title" character varying(150) NOT NULL,
                "Issuer" character varying(150) NOT NULL,
                "IssueYear" integer NOT NULL,
                "Description" character varying(1000) NOT NULL,
                "FileUrl" character varying(500) NULL,
                "FileType" character varying(20) NULL,
                "CreatedAt" timestamp with time zone NOT NULL
            );
            """);
    }
}

app.UseCors("PortfolioFrontend");
app.UseStaticFiles();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "healthy",
    service = "Portfolio API",
    timestamp = DateTimeOffset.UtcNow
}));

app.Run();
