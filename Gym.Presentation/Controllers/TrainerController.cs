using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Gym.Application.DTOs.Trainers;
using Gym.Application.Trainers;
using Gym.Application.DTOs.Common;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[SwaggerTag("Trainer management")]
public class TrainerController : ControllerBase
{
    private readonly TrainerService _service;

    public TrainerController(TrainerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] TrainerQuery query, CancellationToken ct)
    {
        var data = await _service.GetPagedAsync(query, ct);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, includeDeleted: false, ct);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTrainerRequest req, CancellationToken ct)
        => Ok(await _service.CreateAsync(req, ct));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTrainerRequest req, CancellationToken ct)
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