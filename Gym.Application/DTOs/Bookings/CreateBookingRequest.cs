namespace Gym.Application.DTOs.Bookings;

public class CreateBookingRequest
{
    public Guid MemberId { get; set; }
    public Guid SessionId { get; set; }
}