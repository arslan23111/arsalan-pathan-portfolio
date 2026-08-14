using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Certificates;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/certificates")]
public sealed class CertificatesController(CertificateService service, IWebHostEnvironment environment) : ControllerBase
{
    private static readonly Dictionary<string, string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
        ["application/pdf"] = ".pdf"
    };

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var certificate = await service.GetByIdAsync(id, cancellationToken);
        return certificate is null ? NotFound() : Ok(certificate);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CertificateRequest request, CancellationToken cancellationToken)
    {
        var certificate = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = certificate.Id }, certificate);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, CertificateRequest request, CancellationToken cancellationToken) =>
        await service.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File must be between 1 byte and 5 MB." });
        if (!AllowedTypes.TryGetValue(file.ContentType, out var extension))
            return BadRequest(new { message = "Only JPG, PNG, WebP and PDF files are allowed." });

        var folder = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads", "certificates");
        Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
        await file.CopyToAsync(stream, cancellationToken);
        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/certificates/{fileName}";
        return Ok(new { fileUrl, fileType = extension.TrimStart('.') });
    }
}
