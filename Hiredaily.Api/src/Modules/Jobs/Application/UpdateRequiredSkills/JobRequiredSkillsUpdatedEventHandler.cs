using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Jobs.Application.Outbox;
using Hiredaily.Modules.Jobs.Domain.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;

public class JobRequiredSkillsUpdatedEventHandler(IOutboxRepository repository) : IDomainEventHandler<JobRequiredSkillsUpdatedEvent>
{
    public Task Handle(JobRequiredSkillsUpdatedEvent evt)
    {
        var integrationEvent = new JobRequiredSkillsUpdatedMessage
        {
            EventData = new JobSkillsUpdatedMessagePayload(evt.AggregateId, evt.RequiredSkills),
            CreatedAt = DateTime.UtcNow,
            EventId = Guid.NewGuid()
        };
        repository.AddMessage<JobRequiredSkillsUpdatedMessage, JobSkillsUpdatedMessagePayload>(integrationEvent);
        return Task.CompletedTask; throw new NotImplementedException();
    }
}
