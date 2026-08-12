using System.Text;
using Azure.Messaging.ServiceBus;
using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.BuildingBlock.Infra.Messaging.ServiceBus;

public sealed class ServiceBusPublisher(ServiceBusClient client) : IMessagePublisher
{
    private readonly ServiceBusSender _sender = client.CreateSender("jobs");

    public async Task PublishAsync(
        MessageEnvelope envelope,
        CancellationToken cancellationToken = default)
    {
        var id = envelope.Payload
                                .GetType()
                                .GetProperties()
                                .FirstOrDefault(x => x.Name == "Id")
                                ?.GetValue(envelope.Payload) as string;
        var serviceBusMessage = new ServiceBusMessage(
            Encoding.UTF8.GetBytes(envelope.Payload))
        {
            MessageId = id ?? Guid.NewGuid().ToString(),
            Subject = envelope.Payload.GetType().Name
        };

        await _sender.SendMessageAsync(
            serviceBusMessage,
            cancellationToken);
    }
}