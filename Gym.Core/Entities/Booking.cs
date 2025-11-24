namespace Gym.Core.Entities;

public class Booking : BaseEntity
{
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = default!;
    
    public Guid SessionId { get; set; }
    public ClassSession Session { get; set; } = default!;

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Active";
    
    public Attendance? Attendance { get; set; }
}