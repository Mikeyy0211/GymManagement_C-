using Gym.Application.Bookings;
using Gym.Application.DTOs.Bookings;
using Gym.Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[SwaggerTag("Manage class session bookings: create, cancel, and view member bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingService _service;

    public BookingController(BookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest req, CancellationToken ct)
    {
        var result = await _service.CreateAsync(req, ct);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking created"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await _service.CancelAsync(id, ct);
        return NoContent();
    }

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetByMember(Guid memberId, CancellationToken ct)
    {
        var list = await _service.GetByMemberAsync(memberId, ct);
        return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok(list));
    }
}