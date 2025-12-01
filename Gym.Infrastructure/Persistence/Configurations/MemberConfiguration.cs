using Gym.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastructure.Persistence.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> b)
    {
        b.ToTable("Members");
        b.HasKey(x => x.Id);

        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        b.Property(x => ((BaseEntity)x).RowVersion).IsRowVersion();

        b.Property(x => x.FullName).IsRequired().HasMaxLength(100);

        b.HasOne(m => m.MembershipPlan)
            .WithMany()
            .HasForeignKey(m => m.MembershipPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);

        // index phục vụ tìm kiếm nhanh
        b.HasIndex(x => new { x.FullName, x.IsDeleted });
        b.HasIndex(x => x.MembershipPlanId);
    }
}