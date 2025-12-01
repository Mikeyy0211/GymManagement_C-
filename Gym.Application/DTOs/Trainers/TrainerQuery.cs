using Gym.Application.Common;

namespace Gym.Application.DTOs.Trainers;

public class TrainerQuery : PagedQuery
{
    public string? Search { get; set; }
}