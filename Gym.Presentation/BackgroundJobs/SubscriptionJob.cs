using Cronos;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Presentation.BackgroundJobs.Base;

namespace Gym.Presentation.BackgroundJobs;

public class SubscriptionJob : CronJobService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<SubscriptionJob> _logger;

    public SubscriptionJob(IServiceProvider sp, ILogger<SubscriptionJob> logger)
        : base("0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")) // chạy mỗi 00:00 VN
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var scope = _sp.CreateScope();

        var lockRepo = scope.ServiceProvider.GetRequiredService<IJobLockRepository>();
        var jobRepo  = scope.ServiceProvider.GetRequiredService<IJobHistoryRepository>();
        var service  = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

        const string jobName = "SubscriptionJob";

        // ===== 1) Job lock để chống chạy trùng =====
        if (!await lockRepo.TryAcquireLockAsync(jobName, TimeSpan.FromMinutes(30)))
        {
            _logger.LogWarning("Job skipped (already running): {job}", jobName);
            return;
        }

        _logger.LogInformation("Job started: {job}", jobName);

        try
        {
            // ===== 2) Chạy xử lý subscription =====
            await service.ProcessExpiringSubscriptionsAsync();
            await service.ProcessExpiredSubscriptionsAsync();

            _logger.LogInformation("Subscription job completed.");

            // ===== 3) Lưu job history thành công =====
            await jobRepo.AddAsync(new JobHistory
            {
                JobName   = jobName,
                Success   = true,
                Message   = "Completed successfully",
                ExecutedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Subscription job failed");

            // ===== 4) Lưu job history thất bại =====
            await jobRepo.AddAsync(new JobHistory
            {
                JobName   = jobName,
                Success   = false,
                Message   = ex.Message,
                ExecutedAt = DateTime.UtcNow
            });
        }
        finally
        {
            await lockRepo.ReleaseLockAsync(jobName);
            _logger.LogInformation("Job finished: {job}", jobName);
        }
    }
}