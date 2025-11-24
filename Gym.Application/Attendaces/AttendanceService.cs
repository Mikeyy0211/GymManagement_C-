using Gym.Application.DTOs.Attendances;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Application.Attendaces;

public class AttendanceService
{
    private readonly IAttendanceRepository _attRepo;
    private readonly IBookingRepository _bookingRepo;

    public AttendanceService(IAttendanceRepository attRepo, IBookingRepository bookingRepo)
    {
        _attRepo = attRepo;
        _bookingRepo = bookingRepo;
    }

    public async Task<AttendanceDto> CheckInAsync(Guid bookingId)
    {
        var booking = await _bookingRepo.GetByIdAsync(
            bookingId,
            asNoTracking: true,
            includeDeleted: false,
            ct: CancellationToken.None
        );

        var attendance = new Attendance
        {
            BookingId = bookingId,
            Status = AttendanceStatus.Present,
            CheckInTime = DateTime.UtcNow
        };

        await _attRepo.AddAsync(attendance, CancellationToken.None);

        return new AttendanceDto(
            attendance.Id,
            attendance.Status,
            attendance.CheckInTime
        );
    }
}