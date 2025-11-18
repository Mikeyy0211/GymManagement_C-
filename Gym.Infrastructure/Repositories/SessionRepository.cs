using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class SessionRepository : BaseRepository<ClassSession>, ISessionRepository
{
    public SessionRepository(GymDbContext db) : base(db) {}

    public override IQueryable<ClassSession> Query(bool includeDeleted = false)
    {
        var q = _db.ClassSessions
            .Include(s => s.GymClass!)
            .ThenInclude(c => c.Trainer!)
            .ThenInclude(t => t.User!)
            .AsQueryable();

        if (!includeDeleted)
            q = q.Where(s => !s.IsDeleted);

        return q;
    }

}
