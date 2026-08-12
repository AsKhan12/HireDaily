using System.Text;
using System.Text.Json;
using Hiredaily.BuildingBlock.Application.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Hiredaily.BuildingBlock.Infra.Messaging.RabbitMq;

public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly RabbitmqSettings _rabbitmqSettings;

    public RabbitMqPublisher(IConnection connection, IOptions<RabbitmqSettings> options)
    {
        _connection = connection;
        _rabbitmqSettings = options.Value;
    }

    public async Task PublishAsync(MessageEnvelope message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(_rabbitmqSettings.Exchange);
        ArgumentNullException.ThrowIfNull(message.Payload, $"Incorrect payload, {nameof(message.Payload)}");
        ArgumentException.ThrowIfNullOrEmpty(message.PayloadMetadata?["routing-key"], "routing-key");

        await using var channel = await _connection.CreateChannelAsync(null, cancellationToken);

        var body = Encoding.UTF8.GetBytes(message.Payload);

        await channel.BasicPublishAsync(
            exchange: _rabbitmqSettings.Exchange,
            routingKey: message.PayloadMetadata["routing-key"],
            body: body,
            cancellationToken: cancellationToken);
    }
}