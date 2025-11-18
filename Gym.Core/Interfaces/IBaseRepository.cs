using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    IQueryable<T> Query(bool includeDeleted = false);
    Task<T?> GetByIdAsync(Guid id, bool asNoTracking, bool includeDeleted, CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
    Task SoftDeleteAsync(T entity, CancellationToken ct);
}