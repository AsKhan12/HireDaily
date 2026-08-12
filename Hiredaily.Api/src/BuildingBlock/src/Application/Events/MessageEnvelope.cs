namespace Hiredaily.BuildingBlock.Application.Events;

public sealed record MessageEnvelope(
    string Payload,
    IReadOnlyDictionary<string, string>? PayloadMetadata = null
    );