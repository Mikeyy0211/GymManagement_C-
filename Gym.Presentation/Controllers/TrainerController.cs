using Microsoft.AspNetCore.Mvc;
using Gym.Application.DTOs.Trainers;
using Gym.Application.Trainers;


namespace Gym.Presentation.Controllers;
[ApiController]
[Route("trainers")]
public class TrainerController : ControllerBase
{
    private readonly TrainerService _service;

    public TrainerController(TrainerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

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