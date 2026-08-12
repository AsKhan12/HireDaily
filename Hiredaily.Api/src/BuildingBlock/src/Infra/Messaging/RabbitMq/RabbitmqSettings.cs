namespace Hiredaily.BuildingBlock.Infra.Messaging.RabbitMq;
public sealed class RabbitmqSettings
{
    public const string SectionName = "RabbitMq";
    public string Host { get; init; } = default!;
    public int Port { get; init; }
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string? Exchange { get; init; } = default!;
    public string? Queue { get; init; } = default!;
}
