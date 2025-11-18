using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Plans;
using Gym.Application.Plans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[SwaggerTag("Membership plans management with paging, soft delete, and ETag concurrency.")]
public class PlansController : ControllerBase
{
    private readonly PlanService _service;
    public PlansController(PlanService service) => _service = service;

    [HttpGet]
    [SwaggerOperation(Summary = "List plans", Description = "Supports search, min/max price, sort, paging.")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PlanDto>>), 200)]
    public async Task<IActionResult> Get([FromQuery] PlanQuery query, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(query, ct);
        return Ok(ApiResponse<PagedResult<PlanDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get plan by id", Description = "Returns ETag header; use it in If-Match for update/delete.")]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDeleted, CancellationToken ct)
    {
        try
        {
            var (dto, etag) = await _service.GetByIdAsync(id, includeDeleted, ct);
            if (!string.IsNullOrEmpty(etag))
                Response.Headers.ETag = $"\"{etag}\"";
            return Ok(ApiResponse<PlanDto>.Ok(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Plan not found"));
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a plan", Description = "Returns created plan and ETag header.")]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), 200)]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest req, CancellationToken ct)
    {
        var (dto, etag) = await _service.CreateAsync(req, ct);
        if (!string.IsNullOrEmpty(etag))
            Response.Headers.ETag = $"\"{etag}\"";
        return Ok(ApiResponse<PlanDto>.Ok(dto, "Plan created"));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update a plan",
        Description = "Pass latest ETag in If-Match header (preferred). Fallback: send RowVersionBase64 in body."
    )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(428)] // Precondition Required
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanRequest req, CancellationToken ct)
    {
        try
        {
            var ifMatch = Request.Headers.IfMatch.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ifMatch) && string.IsNullOrWhiteSpace(req.RowVersionBase64))
                return StatusCode(428, ApiResponse<string>.Fail("Provide If-Match ETag or RowVersionBase64"));

            // strip quotes if present
            var etag = ifMatch?.Trim('"');
            await _service.UpdateAsync(id, req, etag, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Plan not found"));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Soft delete a plan (idempotent)",
        Description = "Send ETag in If-Match header. Deleting an already-deleted plan still returns 204."
    )]
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
            return NotFound(ApiResponse<string>.Fail("Plan not found"));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
