using Hiredaily.BuildingBlock.Domain.Entity;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.Modules.JobApplication.Domain.Enum;

namespace Hiredaily.Modules.JobApplication.Domain;

public class JobApplication : IEntity<JobApplicationId>
{
    private List<IDomainEvent> _events = [];
    public JobApplicationId Id {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? UpdatedAt {get; private set;}
    public UserId UserId {get; private set;}
    public JobId JobId {get; private set;}
    public ApplicationStageEnum ApplicationStage {get; private set;}

    public JobApplication(ApplicationStageEnum applicationStage, JobId jobId, UserId userId)
    {
        ApplicationStage = applicationStage;
        JobId = jobId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Id = new JobApplicationId(Guid.NewGuid());
    }

    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();
    public void ClearEvents()
    {
        _events.Clear();
    }
}
