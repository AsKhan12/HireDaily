namespace Hiredaily.BuildingBlock.Infra.Messaging.ServiceBus;

public sealed class ServiceBusSettings
{
    public const string SectionName  = "ServiceBusSettings";
    public required string Connectionstring {get; init;}
    public required string Topic {get; init;}
}