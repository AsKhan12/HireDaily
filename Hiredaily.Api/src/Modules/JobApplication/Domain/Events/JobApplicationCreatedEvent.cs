using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.JobApplication.Domain.Events;

public class JobApplicationCreatedEvent : IDomainEvent
{
    public DateTime CreatedAt {get; } = DateTime.UtcNow;

    public Guid EventId {get; } = Guid.NewGuid();

    public string EventName {get; } = "jobapplication.created";
    
}