using Hiredaily.BuildingBlock.Domain.Entity;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Jobs.Domain.Events;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Domain;

public class Job : IEntity<JobId>
{
    private readonly List<IDomainEvent> _events = [];
    private readonly List<Skill> _requiredSkills = [];

    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();

    private Job()
    {
    }

    public JobId Id { get; private set; } = default!;
    public string Title {get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public OrganizationId OrganizationId { get; private set; } = default!;
    public IReadOnlyList<Skill> RequiredSkills => _requiredSkills.AsReadOnly();
    public JobSite JobSite { get; private set; } = default!;
    public Money HourlyRate { get; private set; } = default!;

    public Job(Money hourlyRate, JobSite jobSite, IReadOnlyList<Skill> requiredSkills, OrganizationId organizationId, string title)
    {
        HourlyRate = hourlyRate ?? throw new ArgumentNullException(nameof(hourlyRate));
        JobSite = jobSite ?? throw new ArgumentNullException(nameof(jobSite));
        _requiredSkills.AddRange(requiredSkills ?? throw new ArgumentNullException(nameof(requiredSkills)));
        OrganizationId = organizationId;
        CreatedAt = DateTime.UtcNow;
        Id = new JobId(Guid.NewGuid());
        Title = title;
        _events.Add(new JobCreatedEvent(
            Id,
            CreatedAt,
            null,
            OrganizationId,
            _requiredSkills,
            JobSite,
            HourlyRate,
            title
        ));
    }

    public void UpdateHourlyRate(Money money)
    {
        HourlyRate = money ?? throw new ArgumentNullException(nameof(money));
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new JobHourlyRateUpdatedEvent(Id, HourlyRate));
    }

    public void UpdateTitle(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new JobTitleUpdatedEvent(Id, Title));
    }

    public void UpdateJobSite(JobSite jobSite)
    {
        JobSite = jobSite ?? throw new ArgumentNullException(nameof(jobSite));
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new JobSiteUpdatedEvent(Id, JobSite));
    }

    public void UpdateRequiredSkills(IReadOnlyList<Skill> skills)
    {
        ArgumentNullException.ThrowIfNull(skills);
        _requiredSkills.Clear();
        _requiredSkills.AddRange(skills);
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new JobRequiredSkillsUpdatedEvent(Id, RequiredSkills));
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}
