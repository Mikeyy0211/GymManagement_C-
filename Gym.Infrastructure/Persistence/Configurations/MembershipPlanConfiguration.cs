using Gym.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastructure.Persistence.Configurations;

public sealed class MembershipPlanConfiguration : IEntityTypeConfiguration<MembershipPlan>
{
    public void Configure(EntityTypeBuilder<MembershipPlan> b)
    {
        b.ToTable("MembershipPlans");
        b.HasKey(x => x.Id);

        // audit & concurrency
        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        b.Property(x => x.RowVersion).IsRowVersion();

        // business
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.Price).HasPrecision(18, 2);
        b.Property(x => x.DurationDays).IsRequired();
        b.Property(x => x.MaxSessionsPerWeek).IsRequired();

        b.HasIndex(x => new { x.Name, x.IsDeleted })
            .HasFilter("[IsDeleted] = 0")
            .IsUnique();

        // soft-delete filter
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
