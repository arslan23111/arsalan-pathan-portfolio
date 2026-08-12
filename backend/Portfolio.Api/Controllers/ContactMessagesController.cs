using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Application.Contacts;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/contact-messages")]
public sealed class ContactMessagesController(CreateContactMessageService service, ContactMessageAdminService adminService) : ControllerBase
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
        var created = await service.ExecuteAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new
        {
            id = created.Id,
            message = "Your message has been received successfully."
        });
    }

    [Authorize(Roles = "Admin"), HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken token) => Ok(await adminService.GetAllAsync(token));

    [Authorize(Roles = "Admin"), HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> SetRead(Guid id, [FromQuery] bool value, CancellationToken token) =>
        await adminService.SetReadStatusAsync(id, value, token) ? NoContent() : NotFound();

    [Authorize(Roles = "Admin"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token) =>
        await adminService.DeleteAsync(id, token) ? NoContent() : NotFound();
}
