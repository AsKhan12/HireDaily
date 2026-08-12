using System.Text.Json;
using Hiredaily.BuildingBlock.Application.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Feed.Application;

public class IntegrationHandlerFactory(IServiceScopeFactory scopeFactory)
{
    public async Task InvokeHandler(string message, string eventName, CancellationToken cancellationToken = default)
    {
        using var scope = scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredKeyedService<IIntegrationEventHandler>(eventName);

        await handler.HandleAsync(message, cancellationToken);
    }
}