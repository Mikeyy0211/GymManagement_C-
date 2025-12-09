using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gym.Presentation.Pages.Admin.Jobs;

public class RunModel : PageModel
{
    private readonly ISubscriptionService _service;
    private readonly IJobHistoryRepository _history;

    public RunModel(ISubscriptionService service, IJobHistoryRepository history)
    {
        _service = service;
        _history = history;
    }

    public async Task<IActionResult> OnPost()
    {
        try
        {
            await _service.ProcessExpiringSubscriptionsAsync();
            await _service.ProcessExpiredSubscriptionsAsync();

            await _history.AddSuccessAsync("SubscriptionJob", "Manual run OK");

            TempData["msg"] = "Job executed successfully!";
        }
        catch (Exception ex)
        {
            await _history.AddFailAsync("SubscriptionJob", ex.Message);
            TempData["msg"] = "Job failed: " + ex.Message;
        }

        return Redirect("/admin/jobs");
    }
}