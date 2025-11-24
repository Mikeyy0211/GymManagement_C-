namespace Gym.Application.DTOs.Bookings;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Guid SessionId { get; set; }
    public DateTime BookedAt { get; set; }
    public string Status { get; set; } = "";
}