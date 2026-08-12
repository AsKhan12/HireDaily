namespace Hiredaily.Modules.Feed.Application.Common;
using System.Text.Json.Serialization;
public sealed class JobFeed
{
    private JobFeed()
    {
    }
    [JsonPropertyName("timestamp")] 
    public DateTime Timestamp { get; private set; }
    [JsonPropertyName("jobId")]
    public JobId JobId { get; private set; } = default!;
    [JsonPropertyName("title")]
    public string Title { get; private set; } = default!;
    [JsonPropertyName("organizationId")]
    public OrganizationId OrganizationId { get; private set; } = default!;
    [JsonPropertyName("jobCreatedAt")]
    public DateTime JobCreatedAt { get; private set; }
    [JsonPropertyName("jobLastUpdatedAt")]
    public DateTime? JobLastUpdatedAt { get; private set; }
    [JsonPropertyName("hourlyRate")]
    public Money HourlyRate { get; private set; } = default!;
    [JsonPropertyName("jobSite")]
    public JobSite JobSite { get; private set; } = default!;
    [JsonPropertyName("requiredSkills")]
    public IReadOnlyList<Skill> RequiredSkills { get; private set; } = [];
    [JsonPropertyName("isActive")]
    public bool IsActive { get; private set; }

    public static JobFeed Create(
        DateTime timestamp,
        JobId jobId,
        string title,
        DateTime jobCreatedAt,
        DateTime? jobLastUpdatedAt,
        OrganizationId organizationId,
        Money hourlyRate,
        JobSite jobSite,
        IReadOnlyList<Skill> requiredSkills)
    {
        return new JobFeed
        {
            Title = title,
            Timestamp = timestamp,
            JobId = jobId,
            JobCreatedAt = jobCreatedAt,
            JobLastUpdatedAt = jobLastUpdatedAt,
            OrganizationId = organizationId,
            HourlyRate = hourlyRate,
            JobSite = jobSite,
            RequiredSkills = requiredSkills,
            IsActive = true
        };
    }
}
