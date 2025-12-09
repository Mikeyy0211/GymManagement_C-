using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/job-test")]
public class JobTestController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public JobTestController(ISubscriptionService service)
    {
        _service = service;
    }

    [HttpPost("subscription")]
    public async Task<IActionResult> RunSubscriptionJob()
    {
        await _service.ProcessExpiringSubscriptionsAsync();
        await _service.ProcessExpiredSubscriptionsAsync();

        return Ok(new { message = "Subscription job executed manually!" });
    }
}