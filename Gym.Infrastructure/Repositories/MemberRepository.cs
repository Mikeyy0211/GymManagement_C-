using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class MemberRepository : BaseRepository<Member>, IMemberRepository
{
    public MemberRepository(GymDbContext db) : base(db) {}

    public override IQueryable<Member> Query(bool includeDeleted = false)
    {
        var q = _db.Members.Include(m => m.MembershipPlan).AsQueryable();

        if (!includeDeleted)
            q = q.Where(m => !m.IsDeleted);

        return q;
    }
}