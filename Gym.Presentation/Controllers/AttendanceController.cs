using Gym.Application.Attendaces;
using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Attendances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // optional — tuỳ bạn muốn chỉ Member/Admin mới checkin
[SwaggerTag("Manage check-in / attendance for gym classes.")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _service;

    public AttendanceController(AttendanceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Check-in 1 booking (member đến lớp)
    /// </summary>
    [HttpPost("checkin/{bookingId:guid}")]
    [SwaggerOperation(Summary = "Check-in a member", 
        Description = "Mark attendance as Present for a booking.")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CheckIn(Guid bookingId, CancellationToken ct)
    {
        try
        {
            var dto = await _service.CheckInAsync(bookingId);
            return Ok(ApiResponse<AttendanceDto>.Ok(dto, "Checked in successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Booking not found"));
        }
    }
}