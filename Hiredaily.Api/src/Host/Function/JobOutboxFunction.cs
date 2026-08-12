using Hiredaily.Modules.Jobs.Application.Outbox;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Host.Function;

public class JobOutboxFunction(
    ILoggerFactory loggerFactory,
    JobOutboxMessagePublisher processor)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<JobOutboxFunction>();

    [Function("JobOutboxFunction")]
    public async Task Run(
        [TimerTrigger("* * * * * *")] TimerInfo myTimer,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        # if DEBUG
        await processor.ProcessAsync(CancellationToken.None);
        # else
        await processor.ProcessAsync(CancellationToken.None);
        #endif
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}