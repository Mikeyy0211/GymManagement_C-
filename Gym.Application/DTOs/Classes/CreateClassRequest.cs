namespace Gym.Application.DTOs.Classes;

public class CreateClassRequest
{
    public string Name { get; set; } = default!;
    public Guid? TrainerId { get; set; }
    public int Capacity { get; set; } = 20;
}