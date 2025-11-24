using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class BookingRepository : BaseRepository<Booking>, IBookingRepository
{
    public BookingRepository(GymDbContext db) : base(db)
    {
    }

    public Task<bool> ExistsAsync(Guid memberId, Guid sessionId, CancellationToken ct)
        => _db.Bookings.AnyAsync(x =>
                x.MemberId == memberId &&
                x.SessionId == sessionId &&
                x.Status == "Active",
            ct
        );

}