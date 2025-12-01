namespace Gym.Application.DTOs.Reports;

public interface IReportService
{
    Task<RevenueReportDto> GetRevenueReportAsync();
    Task<IEnumerable<ClassUtilizationDto>> GetClassUtilizationAsync();
    Task<IEnumerable<TrainerPerformanceDto>> GetTrainerPerformanceAsync();
    Task<MemberActivityDto> GetMemberActivityAsync(Guid memberId);
    
    Task<byte[]> ExportRevenueCsvAsync();
    
}