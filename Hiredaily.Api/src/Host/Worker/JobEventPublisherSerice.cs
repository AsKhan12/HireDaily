using Hiredaily.Modules.Jobs.Application.Outbox;
namespace Worker;

public sealed class JobEventPublisherSerice(
    ILogger<JobEventPublisherSerice> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting Job Events Consumer");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                using var scope  = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<JobOutboxMessagePublisher>();
                await processor.ProcessAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                await ExecuteAsync(stoppingToken);
            }
        }
    }
}