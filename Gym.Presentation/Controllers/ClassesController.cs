using Gym.Application.Classes;
using Gym.Application.DTOs.Classes;
using Gym.Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Trainer")]
public class ClassesController : ControllerBase
{
    private readonly ClassService _service;

    public ClassesController(ClassService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<ClassDto>>.Ok(list));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassRequest rq, CancellationToken ct)
    {
        var dto = await _service.CreateClassAsync(rq, ct);
        return Ok(ApiResponse<ClassDto>.Ok(dto));
    }

    [HttpGet("{classId:guid}/sessions")]
    public async Task<IActionResult> GetSessions(Guid classId, CancellationToken ct)
    {
        var data = await _service.GetSessionsAsync(classId, ct);
        return Ok(ApiResponse<IEnumerable<SessionDto>>.Ok(data));
    }

    [HttpPost("{classId:guid}/sessions")]
    public async Task<IActionResult> CreateSession(Guid classId, CreateSessionRequest rq, CancellationToken ct)
    {
        rq.ClassId = classId;
        var dto = await _service.CreateSessionAsync(rq, ct);
        return Ok(ApiResponse<SessionDto>.Ok(dto));
    }
}