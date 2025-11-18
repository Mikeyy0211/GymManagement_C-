using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly GymDbContext _db;

    public BaseRepository(GymDbContext db)
    {
        _db = db;
    }

    public virtual IQueryable<T> Query(bool includeDeleted = false)
    {
        var q = _db.Set<T>().AsQueryable();
        if (!includeDeleted)
            q = q.Where(x => !x.IsDeleted);

        return q;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking, bool includeDeleted, CancellationToken ct)
    {
        var q = _db.Set<T>().AsQueryable();

        if (!includeDeleted)
            q = q.Where(x => !x.IsDeleted);

        if (asNoTracking)
            q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct)
    {
        await _db.Set<T>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public virtual async Task SoftDeleteAsync(T entity, CancellationToken ct)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }
}