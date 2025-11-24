namespace Gym.Core.Entities;

public class Attendance : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = default!;
    
    public AttendanceStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late
}