using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IBookingRepository : IBaseRepository<Booking>
{
    Task<bool> ExistsAsync(Guid memberId, Guid sessionId, CancellationToken ct);
}