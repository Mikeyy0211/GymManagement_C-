using Gym.Application.Common;
using Gym.Application.DTOs.Sessions;
using Gym.Application.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SessionsController : ControllerBase
{
    private readonly SessionService _service;

    public SessionsController(SessionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] PagedQuery q, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(q, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDeleted, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, includeDeleted, ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassSessionRequest req, CancellationToken ct)
        => Ok(await _service.CreateAsync(req, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClassSessionRequest req, CancellationToken ct)
    {
        await _service.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        await _service.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}