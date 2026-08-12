namespace Hiredaily.Modules.Feed.Application.Common;

public interface IJobFeedRepository
{
    Task Insert(JobFeed feed, CancellationToken cancellationToken = default);
    Task UpdateHourlyRate(JobId jobId, DateTime timestamp, Money hourlyRate, CancellationToken cancellationToken = default);
    Task UpdateTitle(JobId jobId, string title, DateTime timestamp, CancellationToken cancellationToken = default);
    Task UpdateJobSite(JobId jobId, DateTime timestamp, JobSite jobSite, CancellationToken cancellationToken = default);
    Task UpdateRequiredSkills(JobId jobId, DateTime timestamp, IReadOnlyList<Skill> requiredSkills, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobFeed>> GetJobFeed(Location? location, List<string>? skills, CancellationToken cancellationToken = default);
}
