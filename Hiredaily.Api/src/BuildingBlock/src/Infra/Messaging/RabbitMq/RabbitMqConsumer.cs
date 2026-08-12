using System.Text;
using Hiredaily.BuildingBlock.Application.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hiredaily.BuildingBlock.Infra.Messaging.RabbitMq;

public sealed class RabbitMqConsumer : IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly RabbitmqSettings _rabbitmqSettings;
    private readonly ILogger<RabbitMqConsumer> _logger;
    public RabbitMqConsumer(IConnection connection, IOptions<RabbitmqSettings> options, ILogger<RabbitMqConsumer> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.Queue);
        _rabbitmqSettings = options.Value;
        _connection = connection;
        _logger = logger;
    }

    public async Task SubscribeAsync(Func<string, string, CancellationToken, Task> handler, CancellationToken stoppingToken = default)
    {
        var channel = await _connection.CreateChannelAsync(null, stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _rabbitmqSettings.Queue!,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("{message}", message);

                await handler(message, args.RoutingKey, stoppingToken);

                await channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in processing new queue message: {DeliveryTag}", args.DeliveryTag);

                await channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: false); // or false
            }
        };

        await channel.BasicConsumeAsync(
            queue: _rabbitmqSettings.Queue!,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}