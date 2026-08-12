namespace Hiredaily.Modules.Jobs.Application.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public required string EventType { get; set; }
    public required string Payload { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int RetryCount {get; set;}
    public string? Error {get; set;}
}
