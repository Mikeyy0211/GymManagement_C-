using Gym.Application.Attendaces;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/attendance")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _service;

    public AttendanceController(AttendanceService service)
    {
        _service = service;
    }

    [HttpPost("checkin/{bookingId}")]
    public async Task<IActionResult> CheckIn(Guid bookingId)
        => Ok(await _service.CheckInAsync(bookingId));
}