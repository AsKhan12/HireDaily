namespace Hiredaily.Modules.Feed.Application;

public interface IIntegrationEventHandler
{
    Task HandleAsync(string message, CancellationToken cancellationToken = default);
}