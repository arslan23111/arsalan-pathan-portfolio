using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/contact-messages")]
public sealed class ContactMessagesController(IContactMessageRepository repository) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("ContactForm")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Create(
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

        var created = await repository.CreateAsync(message, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new
        {
            id = created.Id,
            message = "Your message has been received successfully."
        });
    }
}
