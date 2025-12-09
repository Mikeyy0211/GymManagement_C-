using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gym.Presentation.Pages.Admin.Jobs;

public class IndexModel : PageModel
{
    private readonly IJobHistoryRepository _historyRepo;
    private readonly IJobLockRepository _lockRepo;

    public bool IsLocked { get; set; }
    public DateTime? LockTime { get; set; }

    public JobHistory? LastRun { get; set; }

    public IndexModel(IJobHistoryRepository historyRepo, IJobLockRepository lockRepo)
    {
        _historyRepo = historyRepo;
        _lockRepo = lockRepo;
    }

    public async Task OnGet()
    {
        IsLocked = await _lockRepo.IsLockedAsync("SubscriptionJob");
        LockTime = await _lockRepo.GetLockTimeAsync("SubscriptionJob");

        LastRun = await _historyRepo.GetLastRunAsync("SubscriptionJob");
    }
}