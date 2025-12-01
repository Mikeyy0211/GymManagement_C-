using Gym.Application.DTOs.Bookings;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Bookings;

public class BookingService
{
    private readonly IBookingRepository _repo;
    private readonly IMemberRepository _members;
    private readonly ISessionRepository _sessions;

    public BookingService(IBookingRepository repo, IMemberRepository members, ISessionRepository sessions)
    {
        _repo = repo;
        _members = members;
        _sessions = sessions;
    }
    public async Task<BookingDto> CreateAsync(CreateBookingRequest req, CancellationToken ct)
    {
        var member = await _members.GetByIdAsync(req.MemberId, true, false, ct)
                     ?? throw new KeyNotFoundException("Member not found");

        var session = await _sessions.GetByIdAsync(req.SessionId, true, false, ct)
                      ?? throw new KeyNotFoundException("Session not found");

        if (await _repo.ExistsAsync(req.MemberId, req.SessionId, ct))
            throw new InvalidOperationException("Already booked");

        int baseCapacity = session.GymClass?.Capacity ?? 0;
        int capacity = session.CapacityOverride ?? baseCapacity;

        int count = await _repo.Query()
            .CountAsync(x => x.SessionId == req.SessionId && x.Status == "Active", ct);

        if (count >= capacity)
            throw new InvalidOperationException("Session is full");

        var booking = new Booking
        {
            MemberId = req.MemberId,
            SessionId = req.SessionId,
            Status = "Active"
        };

        await _repo.AddAsync(booking, ct);

        return new BookingDto
        {
            Id = booking.Id,
            MemberId = booking.MemberId,
            SessionId = booking.SessionId,
            BookedAt = booking.BookedAt,
            Status = booking.Status
        };
    }

    public async Task CancelAsync(Guid id, CancellationToken ct)
    {
        var booking = await _repo.GetByIdAsync(id, false, false, ct)
                      ?? throw new KeyNotFoundException("Booking not found");

        booking.Status = "Cancelled";

        await _repo.UpdateAsync(booking, ct);
    }

    public async Task<IEnumerable<BookingDto>> GetByMemberAsync(Guid memberId, CancellationToken ct)
    {
        var items = await _repo.Query()
            .Where(x => x.MemberId == memberId)
            .ToListAsync(ct);

        return items.Select(x => new BookingDto
        {
            Id = x.Id,
            MemberId = x.MemberId,
            SessionId = x.SessionId,
            Status = x.Status,
            BookedAt = x.BookedAt
        });
    }

}