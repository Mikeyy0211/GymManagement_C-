using System.ComponentModel.DataAnnotations;

namespace Gym.Core.Entities;
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    // audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // soft-delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // optimistic concurrency
    [Timestamp]  
    public byte[]? RowVersion { get; set; } 
}