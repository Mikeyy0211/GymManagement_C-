using System.Globalization;
using System.Text;
using Gym.Application.DTOs.Reports;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Gym.Application.Reports;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    private readonly IMemoryCache _cache;

    public ReportService(IReportRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    // 1. REVENUE REPORT (CACHED)
    public async Task<RevenueReportDto> GetRevenueReportAsync()
    {
        return await _cache.GetOrCreateAsync("report_revenue", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var payments = await _repo.PaymentsQuery().ToListAsync();

            var monthly = payments
                .GroupBy(p => new { p.PaidAt.Year, p.PaidAt.Month })
                .Select(g => new RevenueByMonthDto(
                    g.Key.Year,
                    g.Key.Month,
                    g.Sum(x => x.Amount)
                ));

            return new RevenueReportDto(
                TotalRevenue: payments.Sum(p => p.Amount),
                TotalPayments: payments.Count,
                SuccessfulPayments: payments.Count(p => p.Status == PaymentStatus.Success),
                FailedPayments: payments.Count(p => p.Status == PaymentStatus.Failed),
                MonthlyStats: monthly
            );
        });
    }

    // ============================
    // 2. CLASS UTILIZATION (CACHED)
    // ============================
    public async Task<IEnumerable<ClassUtilizationDto>> GetClassUtilizationAsync()
    {
        return await _cache.GetOrCreateAsync("report_class_util", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var classes = await _repo.ClassesQuery()
                .Include(c => c.Sessions)
                .ToListAsync();

            return classes.Select(c =>
            {
                var totalBookings = _repo.BookingsQuery()
                    .Include(b => b.Session)
                    .Count(b => b.Session.GymClassId == c.Id);

                var attended = _repo.BookingsQuery()
                    .Include(b => b.Attendance)
                    .Count(b => b.Attendance != null &&
                                b.Attendance.Status == AttendanceStatus.Present);

                return new ClassUtilizationDto(
                    c.Id,
                    c.Name,
                    c.Capacity,
                    totalBookings,
                    attended,
                    c.Capacity == 0 ? 0 : (double)attended / c.Capacity
                );
            }).ToList();
        });
    }

    // ============================
    // 3. TRAINER PERFORMANCE (CACHED)
    // ============================
    public async Task<IEnumerable<TrainerPerformanceDto>> GetTrainerPerformanceAsync()
    {
        return await _cache.GetOrCreateAsync("report_trainer_perf", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var trainers = await _repo.TrainersQuery().ToListAsync();

            return trainers.Select(t =>
            {
                var classes = _repo.ClassesQuery()
                    .Where(c => c.TrainerId == t.Id)
                    .ToList();

                var classIds = classes.Select(c => c.Id).ToList();

                var totalBookings = _repo.BookingsQuery()
                    .Include(b => b.Session)
                    .Count(b => classIds.Contains(b.Session.GymClassId));

                var attendance = _repo.BookingsQuery()
                    .Include(b => b.Attendance)
                    .Count(b => b.Attendance != null &&
                                b.Attendance.Status == AttendanceStatus.Present);

                return new TrainerPerformanceDto(
                    t.Id,
                    t.Specialty,
                    classes.Count,
                    totalBookings,
                    attendance,
                    classes.Count == 0 ? 0 : (double)attendance / classes.Count
                );
            }).ToList();
        });
    }

    // ============================
    // 4. MEMBER ACTIVITY (NO CACHE)
    // ============================
    public async Task<MemberActivityDto> GetMemberActivityAsync(Guid memberId)
    {
        var bookings = await _repo.BookingsQuery()
            .Where(b => b.MemberId == memberId)
            .Include(b => b.Attendance)
            .ToListAsync();

        return new MemberActivityDto(
            memberId,
            "Unknown",
            bookings.Count,
            bookings.Count(b => b.Attendance?.Status == AttendanceStatus.Present),
            bookings.Count(b => b.Attendance?.Status == AttendanceStatus.Absent),
            0
        );
    }

    // ============================
    // CSV EXPORT (KHÔNG CACHE)
    // ============================
    public async Task<byte[]> ExportRevenueCsvAsync()
    {
        var payments = await _repo.PaymentsQuery()
            .OrderBy(p => p.PaidAt)
            .Select(p => new
            {
                p.Id,
                MemberName = p.Member.FullName,
                p.Amount,
                p.Status,
                Date = p.PaidAt
            })
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("PaymentId,Member,Amount,Status,PaidAt");

        foreach (var p in payments)
        {
            sb.AppendLine(
                $"{p.Id}," +
                $"{p.MemberName}," +
                $"{p.Amount.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.Status}," +
                $"{p.Date:yyyy-MM-dd HH:mm:ss}"
            );
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}