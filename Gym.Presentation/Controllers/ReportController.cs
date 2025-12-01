using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Reports;
using Gym.Application.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;
[SwaggerTag("Analytics & business intelligence reports: revenue, class usage, trainer KPIs, member activity.")]
[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _service;

    public ReportController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue()
        => Ok(ApiResponse<RevenueReportDto>.Ok(await _service.GetRevenueReportAsync()));

    [HttpGet("class-utilization")]
    public async Task<IActionResult> ClassUtilization()
        => Ok(ApiResponse<IEnumerable<ClassUtilizationDto>>.Ok(await _service.GetClassUtilizationAsync()));

    [HttpGet("trainer-performance")]
    public async Task<IActionResult> TrainerPerformance()
        => Ok(ApiResponse<IEnumerable<TrainerPerformanceDto>>.Ok(await _service.GetTrainerPerformanceAsync()));

    [HttpGet("member-activity/{id}")]
    public async Task<IActionResult> MemberActivity(Guid id)
        => Ok(ApiResponse<MemberActivityDto>.Ok(await _service.GetMemberActivityAsync(id)));
    
    [HttpGet("revenue/export-csv")]
    public async Task<IActionResult> ExportRevenueCsv()
    {
        var fileBytes = await _service.ExportRevenueCsvAsync();

        return File(
            fileBytes,
            "text/csv",
            $"revenue_report_{DateTime.UtcNow:yyyyMMddHHmmss}.csv"
        );
    }
    [HttpPost("clear-cache")]
    [Authorize(Roles = "Admin")]
    public IActionResult ClearCache([FromServices] IMemoryCache cache)
    {
        (cache as MemoryCache)?.Compact(1.0); // clear 100%
        return Ok("Cache cleared!");
    }
}