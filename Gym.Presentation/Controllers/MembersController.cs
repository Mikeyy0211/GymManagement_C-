using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Members;
using Gym.Application.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/members")]
[Authorize(Roles = "Admin")]
[SwaggerTag("Members management (CRUD, assign membership plan).")]
public class MembersController : ControllerBase
{
    private readonly MemberService _service;

    public MembersController(MemberService service)
    {
        _service = service;
    }

    // GET Paged
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] MemberQuery query, CancellationToken ct)
    {
        var data = await _service.GetPagedAsync(query, ct);
        return Ok(ApiResponse<PagedResult<MemberDto>>.Ok(data));
    }

    // GET by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDeleted, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, includeDeleted, ct);
        return Ok(ApiResponse<MemberDto>.Ok(dto));
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberRequest req, CancellationToken ct)
    {
        var dto = await _service.CreateAsync(req, ct);
        return Ok(ApiResponse<MemberDto>.Ok(dto, "Member created"));
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateMemberRequest req, CancellationToken ct)
    {
        await _service.UpdateAsync(id, req, ct);
        return NoContent();
    }

    // DELETE (soft)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.SoftDeleteAsync(id, ct);
        return NoContent();
    }

    // ASSIGN Plan
    [HttpPost("{id:guid}/assign-plan/{planId:guid}")]
    public async Task<IActionResult> AssignPlan(Guid id, Guid planId, CancellationToken ct)
    {
        await _service.AssignPlanAsync(id, planId, ct);
        return Ok(ApiResponse<string>.Ok("Plan assigned"));
    }
}