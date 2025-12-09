using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/admin/jobs")]
public class AdminJobController : ControllerBase
{
    private readonly IJobHistoryRepository _historyRepo;

    public AdminJobController(IJobHistoryRepository historyRepo)
    {
        _historyRepo = historyRepo;
    }

    /// <summary>
    /// Lấy lịch sử chạy job (mặc định lấy 20 lần gần nhất)
    /// </summary>
    [HttpGet("history/{jobName}")]
    public async Task<IActionResult> GetHistory(string jobName, int count = 20)
    {
        var logs = await _historyRepo.GetHistoryAsync(jobName);

        // chỉ trả về số lượng theo count
        logs = logs.Take(count).ToList();

        return Ok(logs);
    }

    /// <summary>
    /// Lấy lần chạy cuối cùng
    /// </summary>
    [HttpGet("last-run/{jobName}")]
    public async Task<IActionResult> GetLastRun(string jobName)
    {
        var last = await _historyRepo.GetLastRunAsync(jobName);
        return Ok(last);
    }
}