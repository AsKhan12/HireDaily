using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Read;

public class JobFeedResponse
{
    public JobId AggregateId { get;  set; } = default!;

    public DateTime Timestamp { get;  set; }

    public JobId JobId { get;  set; } = default!;

    public OrganizationId OrganizationId { get;  set; } = default!;

    public DateTime JobCreatedAt { get;  set; }

    public DateTime? JobLastUpdatedAt { get;  set; }

    public Money HourlyRate { get;  set; } = default!;

    public JobSite JobSite { get;  set; } = default!;

    public IReadOnlyList<Skill> RequiredSkills { get;  set; } = [];

    public bool IsActive { get;  set; }
}