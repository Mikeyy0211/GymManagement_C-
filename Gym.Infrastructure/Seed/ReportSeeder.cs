using Gym.Core.Entities;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Seed;

public static class ReportSeeder
{
    public static async Task SeedReportDataAsync(GymDbContext db)
    {
        Console.WriteLine(">>> Running ReportSeeder...");

        // Nếu đã seed rồi thì bỏ qua
        if (await db.Payments.AnyAsync())
        {
            Console.WriteLine(">>> ReportSeeder skipped (Payments already exist).");
            return;
        }

        // Lấy admin user đã seed trong DataSeeder
        var adminUser = await db.Users.FirstAsync();

        // ==== 1) Trainer Profile ====
        var trainer = new TrainerProfile
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,    // FIX FOREIGN KEY
            Specialty = "Fitness",
            ExperienceYears = 5,
            Phone = "0123456789"
        };

        // ==== 2) Membership Plan ====
        var plan = new MembershipPlan
        {
            Id = Guid.NewGuid(),
            Name = "Basic Plan",
            Price = 499_000,
            DurationDays = 30,
            CreatedAt = DateTime.UtcNow
        };

        // ==== 3) Members ====
        var member1 = new Member
        {
            Id = Guid.NewGuid(),
            FullName = "Nguyen Van A",
            DateOfBirth = new DateTime(1995, 3, 10),
            MembershipPlanId = plan.Id
        };

        var member2 = new Member
        {
            Id = Guid.NewGuid(),
            FullName = "Tran Thi B",
            DateOfBirth = new DateTime(1998, 6, 22),
            MembershipPlanId = plan.Id
        };

        // ==== 4) Gym Class ====
        var gymClass = new GymClass
        {
            Id = Guid.NewGuid(),
            Name = "Yoga Class",
            TrainerId = trainer.Id,
            Capacity = 20
        };

        // ==== 5) Class Sessions ====
        var session1 = new ClassSession
        {
            Id = Guid.NewGuid(),
            GymClassId = gymClass.Id,
            StartAt = DateTime.UtcNow.AddDays(-10),
            EndAt = DateTime.UtcNow.AddDays(-10).AddHours(1)
        };

        var session2 = new ClassSession
        {
            Id = Guid.NewGuid(),
            GymClassId = gymClass.Id,
            StartAt = DateTime.UtcNow.AddDays(-5),
            EndAt = DateTime.UtcNow.AddDays(-5).AddHours(1)
        };

        // ==== 6) Payments ====
        var payment1 = new Payment
        {
            Id = Guid.NewGuid(),
            MemberId = member1.Id,
            PlanId = plan.Id,
            Amount = 499_000,
            PaidAt = DateTime.UtcNow.AddDays(-20),
            ExpireAt = DateTime.UtcNow.AddDays(10),
            Status = PaymentStatus.Success
        };

        var payment2 = new Payment
        {
            Id = Guid.NewGuid(),
            MemberId = member2.Id,
            PlanId = plan.Id,
            Amount = 499_000,
            PaidAt = DateTime.UtcNow.AddDays(-15),
            ExpireAt = DateTime.UtcNow.AddDays(15),
            Status = PaymentStatus.Success
        };

        // ==== 7) Bookings ====
        var booking1 = new Booking
        {
            Id = Guid.NewGuid(),
            MemberId = member1.Id,
            SessionId = session1.Id,
            Status = "Active"
        };

        var booking2 = new Booking
        {
            Id = Guid.NewGuid(),
            MemberId = member2.Id,
            SessionId = session2.Id,
            Status = "Active"
        };

        // ==== 8) Attendance ====
        var attendance1 = new Attendance
        {
            Id = Guid.NewGuid(),
            BookingId = booking1.Id,
            Status = AttendanceStatus.Present,
            CheckInTime = DateTime.UtcNow.AddDays(-10).AddMinutes(5)
        };

        var attendance2 = new Attendance
        {
            Id = Guid.NewGuid(),
            BookingId = booking2.Id,
            Status = AttendanceStatus.Absent
        };

        // ==== ADD TO DB ====
        db.TrainerProfiles.Add(trainer);
        db.MembershipPlans.Add(plan);
        db.Members.AddRange(member1, member2);
        db.GymClasses.Add(gymClass);
        db.ClassSessions.AddRange(session1, session2);
        db.Payments.AddRange(payment1, payment2);
        db.Bookings.AddRange(booking1, booking2);
        db.Attendances.AddRange(attendance1, attendance2);

        await db.SaveChangesAsync();

        Console.WriteLine(">>> ReportSeeder completed successfully!");
    }
}