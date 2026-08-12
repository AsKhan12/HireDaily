namespace Hiredaily.BuildingBlock.Application.Events;

using Hiredaily.BuildingBlock.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
public class IntegrationEventDispatcher(IServiceProvider serviceProvider)
{
    public async Task Dispatch<TEvent>(TEvent evt) where TEvent : class, IDomainEvent
    {
        var handlerType = typeof(IDomainEventHandler<>)
            .MakeGenericType(evt.GetType());
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        await handler.Handle((dynamic) evt);        
    }
}