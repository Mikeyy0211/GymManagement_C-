using Gym.Application.Bookings;
using Gym.Application.DTOs.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingService _service;

    public BookingController(BookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest req, CancellationToken ct)
        => Ok(await _service.CreateAsync(req, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await _service.CancelAsync(id, ct);
        return NoContent();
    }

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetByMember(Guid memberId, CancellationToken ct)
        => Ok(await _service.GetByMemberAsync(memberId, ct));
}