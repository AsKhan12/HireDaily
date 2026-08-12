namespace Hiredaily.BuildingBlock.Application.Events;

public interface IMessageConsumer
{
    Task SubscribeAsync(
        Func<string, string, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}