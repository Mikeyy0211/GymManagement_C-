using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;

namespace Gym.Infrastructure.Repositories;

public class PlanRepository : BaseRepository<MembershipPlan>, IPlanRepository
{
    public PlanRepository(GymDbContext db) : base(db) {}
}