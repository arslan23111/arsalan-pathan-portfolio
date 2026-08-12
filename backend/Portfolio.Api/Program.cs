using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Services;
using Portfolio.Application.Abstractions;
using Portfolio.Application.Contacts;
using Portfolio.Application.Projects;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

builder.Services.AddControllers();
builder.Services.AddScoped<AdminCredentialValidator>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "portfolio_admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<CreateContactMessageService>();
builder.Services.AddScoped<ContactMessageAdminService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortfolioDatabase")));

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
}

app.UseCors("PortfolioFrontend");
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
