using Gym.Application.Classes;
using Gym.Application.DTOs.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/classes")]
[Authorize(Roles = "Admin,Trainer")]
public class ClassController : ControllerBase
{
    private readonly ClassService _service;

    public ClassController(ClassService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] ClassQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, false, ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassRequest req, CancellationToken ct)
        => Ok(await _service.CreateAsync(req, ct));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateClassRequest req, CancellationToken ct)
    {
        await _service.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}