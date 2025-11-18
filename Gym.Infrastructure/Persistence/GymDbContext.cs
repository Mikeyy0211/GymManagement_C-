using System.Reflection;
using Gym.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Persistence;

public class GymDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<GymClass> GymClasses => Set<GymClass>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
    public DbSet<TrainerProfile> TrainerProfiles => Set<TrainerProfile>();
    
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        //row version
        b.Entity<Member>()
            .Property(e => e.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
        b.Entity<MembershipPlan>()
            .Property(e => e.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
        b.Entity<ClassSession>()
            .Property(e => e.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
        b.Entity<GymClass>()
            .Property(e => e.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
        b.Entity<TrainerProfile>()
            .Property(e => e.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();


        // 2) UNIQUE CONSTRAINTS
        b.Entity<TrainerProfile>()
            .HasIndex(t => t.UserId)
            .IsUnique();

        // 3) RELATIONSHIPS
        // Member → Plan (optional, OnDelete SET NULL)
        b.Entity<Member>()
            .HasOne(m => m.MembershipPlan)
            .WithMany()
            .HasForeignKey(m => m.MembershipPlanId)
            .OnDelete(DeleteBehavior.SetNull);

        // GymClass → Trainer (required, OnDelete RESTRICT)
        b.Entity<GymClass>()
            .HasOne(c => c.Trainer)
            .WithMany()
            .HasForeignKey(c => c.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        // GymClass → ClassSessions (Cascade)
        b.Entity<GymClass>()
            .HasMany(c => c.Sessions)
            .WithOne(s => s.GymClass)
            .HasForeignKey(s => s.GymClassId)
            .OnDelete(DeleteBehavior.Cascade);
        
        b.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Deleted:
                    // Convert DELETE => SOFT DELETE
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

