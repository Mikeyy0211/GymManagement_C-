using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gym.Presentation.Pages.Admin.Jobs;

public class HistoryModel : PageModel
{
    private readonly IJobHistoryRepository _repo;

    public HistoryModel(IJobHistoryRepository repo)
    {
        _repo = repo;
    }

    public List<JobHistory> Items { get; set; } = new();
    
    
    public async Task OnGet()
    {
        Items = await _repo.GetHistoryAsync("SubscriptionJob");
    }
}