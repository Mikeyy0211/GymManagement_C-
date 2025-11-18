using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Members;
using Gym.Application.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[SwaggerTag("Members management with paging/filter, ETag concurrency, soft delete, and assign plan.")]
public class MembersController : ControllerBase
{
    private readonly MemberService _service;
    public MembersController(MemberService service) => _service = service;

    [HttpGet]
    [SwaggerOperation(Summary = "List members", Description = "Search by name, filter by plan/date of birth, sort & paging.")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MemberDto>>), 200)]
    public async Task<IActionResult> Get([FromQuery] MemberQuery query, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(query, ct);
        return Ok(ApiResponse<PagedResult<MemberDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get member by id", Description = "Returns ETag header; use it in If-Match for update/delete/assign plan.")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDeleted, CancellationToken ct)
    {
        try
        {
            var (dto, etag) = await _service.GetByIdAsync(id, includeDeleted, ct);
            if (!string.IsNullOrEmpty(etag)) Response.Headers.ETag = $"\"{etag}\"";
            return Ok(ApiResponse<MemberDto>.Ok(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Member not found"));
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a member", Description = "Optionally attach a plan at creation.")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), 200)]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest req, CancellationToken ct)
    {
        var (dto, etag) = await _service.CreateAsync(req, ct);
        if (!string.IsNullOrEmpty(etag)) Response.Headers.ETag = $"\"{etag}\"";
        return Ok(ApiResponse<MemberDto>.Ok(dto, "Member created"));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update a member",
        Description = "Pass latest ETag in If-Match header (preferred). Fallback: RowVersionBase64 in body.")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(428)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberRequest req, CancellationToken ct)
    {
        try
        {
            var ifMatch = Request.Headers.IfMatch.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ifMatch) && string.IsNullOrWhiteSpace(req.RowVersionBase64))
                return StatusCode(428, ApiResponse<string>.Fail("Provide If-Match ETag or RowVersionBase64"));

            var etag = ifMatch?.Trim('"');
            await _service.UpdateAsync(id, req, etag, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Member not found"));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Soft delete a member (idempotent)", Description = "Send ETag in If-Match header.")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(428)]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        try
        {
            var ifMatch = Request.Headers.IfMatch.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ifMatch))
                return StatusCode(428, ApiResponse<string>.Fail("Provide If-Match ETag header"));

            var etag = ifMatch.Trim('"');
            await _service.SoftDeleteAsync(id, etag, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Member not found"));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}/assign-plan")]
    [SwaggerOperation(Summary = "Assign a membership plan to member", Description = "Send RowVersionBase64 in body for concurrency control.")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(428)]
    public async Task<IActionResult> AssignPlan(Guid id, [FromBody] AssignPlanRequest req, CancellationToken ct)
    {
        try
        {
            // Kiểm tra RowVersionBase64 trong body request
            if (string.IsNullOrWhiteSpace(req.RowVersionBase64))
                return StatusCode(428, ApiResponse<string>.Fail("Provide RowVersionBase64"));

            var incomingRowVersion = Convert.FromBase64String(req.RowVersionBase64);

            await _service.AssignPlanAsync(id, req, incomingRowVersion, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
