using Cronos;

namespace Gym.Presentation.BackgroundJobs.Base;

public abstract class CronJobService : BackgroundService
{
    private readonly CronExpression _expression;
    private readonly TimeZoneInfo _timeZone;

    protected CronJobService(string cronExpression, TimeZoneInfo timeZone)
    {
        _expression = CronExpression.Parse(cronExpression);
        _timeZone = timeZone;
    }

    protected abstract Task DoWorkAsync(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZone);

            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;

                if (delay.TotalMilliseconds > 0)
                    await Task.Delay(delay, stoppingToken);
            }

            await DoWorkAsync(stoppingToken);
        }
    }
}