using Microsoft.AspNetCore.Identity;

namespace Gym.Core.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
}