namespace Gym.Application.DTOs.Classes;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid? TrainerId { get; set; }
    public int Capacity { get; set; }

}