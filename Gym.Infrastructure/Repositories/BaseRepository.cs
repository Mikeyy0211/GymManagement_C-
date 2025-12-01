using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly GymDbContext _db;
    protected readonly DbSet<T> _set;

    public BaseRepository(GymDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    // -----------------------------
    // Query
    // -----------------------------
    public virtual IQueryable<T> Query(bool includeDeleted = false)
        => includeDeleted
            ? _set.AsQueryable()
            : _set.Where(e => !e.IsDeleted);

    // -----------------------------
    // Get by ID
    // -----------------------------
    public async Task<T?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        bool includeDeleted = false,
        CancellationToken ct = default)
    {
        var q = Query(includeDeleted);

        if (asNoTracking)
            q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    // -----------------------------
    // Add
    // -----------------------------
    public Task AddAsync(T entity, CancellationToken ct = default)
        => _set.AddAsync(entity, ct).AsTask();

    // -----------------------------
    // Update (with ct)
    // -----------------------------
    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    // -----------------------------
    // Soft Delete (with ct)
    // -----------------------------
    public Task SoftDeleteAsync(T entity, CancellationToken ct = default)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _set.Update(entity);
        return Task.CompletedTask;
    }
}