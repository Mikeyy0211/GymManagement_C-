using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class TrainerRepository : BaseRepository<TrainerProfile>, ITrainerRepository
{
    public TrainerRepository(GymDbContext db) : base(db) {}

    public override IQueryable<TrainerProfile> Query(bool includeDeleted = false)
    {
        var q = _db.TrainerProfiles
            .Include(t => t.User)
            .AsQueryable();

        if (!includeDeleted)
            q = q.Where(t => !t.IsDeleted);

        return q;
    }
}