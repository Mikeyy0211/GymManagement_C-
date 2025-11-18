using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class ClassRepository : BaseRepository<GymClass>, IClassRepository
{
    public ClassRepository(GymDbContext db) : base(db) {}

    public override IQueryable<GymClass> Query(bool includeDeleted = false)
    {
        var q = _db.GymClasses
            .Include(c => c.Trainer)
            .ThenInclude(t => t.User)
            .AsQueryable();

        if (!includeDeleted)
            q = q.Where(c => !c.IsDeleted);

        return q;
    }
}
