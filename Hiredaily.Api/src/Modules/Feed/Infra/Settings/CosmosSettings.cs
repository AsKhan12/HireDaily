namespace Hiredaily.Modules.Feed.Infra.Settings;

public class CosmosSettings
{
    public const string SectionName = "Cosmos";
    public required string Endpoint { get; init; }
    public required string Database { get; init; }
    public required string Container { get; init; }
}