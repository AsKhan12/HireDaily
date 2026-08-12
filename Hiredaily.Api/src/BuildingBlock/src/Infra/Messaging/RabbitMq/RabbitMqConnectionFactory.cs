using RabbitMQ.Client;

namespace Hiredaily.BuildingBlock.Infra.Messaging.RabbitMq;

public sealed class RabbitMqConnectionFactory
{
    public async Task<IConnection> Create(RabbitmqSettings settings)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password
        };

        return await factory.CreateConnectionAsync();
    }
}