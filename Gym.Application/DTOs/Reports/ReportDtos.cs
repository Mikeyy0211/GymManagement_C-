namespace Gym.Application.DTOs.Reports;

public record RevenueReportDto(
    decimal TotalRevenue,
    int TotalPayments,
    int SuccessfulPayments,
    int FailedPayments,
    IEnumerable<RevenueByMonthDto> MonthlyStats
);

public record RevenueByMonthDto(
    int Year,
    int Month,
    decimal Amount
);

public record ClassUtilizationDto(
    Guid ClassId,
    string ClassName,
    int Capacity,
    int TotalBookings,
    int AttendanceCount,
    double AttendanceRate
);

public record TrainerPerformanceDto(
    Guid TrainerId,
    string TrainerName,
    int TotalClasses,
    int TotalBookings,
    int AttendanceCount,
    double AttendanceRate
);

public record MemberActivityDto(
    Guid MemberId,
    string MemberName,
    int TotalBookings,
    int Attended,
    int Missed,
    decimal TotalSpent
);