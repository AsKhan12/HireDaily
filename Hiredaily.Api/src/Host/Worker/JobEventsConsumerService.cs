using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Feed.Application;

namespace Worker;

public sealed class JobEventsConsumerService(
    ILogger<JobEventsConsumerService> logger,
    IntegrationHandlerFactory factory,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting Job Events Consumer");
        using var scope  = scopeFactory.CreateScope();
        var consumer = scope.ServiceProvider.GetService<IMessageConsumer>();
        ArgumentNullException.ThrowIfNull(consumer, "IMessageConsumer not registered.");
        await consumer.SubscribeAsync(
            factory.InvokeHandler,
            stoppingToken);
    }
}