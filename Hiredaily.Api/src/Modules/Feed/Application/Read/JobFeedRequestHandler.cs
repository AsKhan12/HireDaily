using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Read;

public class JobFeedRequestHandler(IJobFeedRepository repository) : IRequestHandler<JobFeedRequest, IEnumerable<JobFeedResponse>>
{
    public async Task<IResult<IEnumerable<JobFeedResponse>>> Handle(JobFeedRequest request, CancellationToken cancellationToken)
    {
        var feed = await repository.GetJobFeed(request.Location, request.Skills, cancellationToken);
        var response = feed.Select(x => new JobFeedResponse
        {
            HourlyRate = x.HourlyRate,
            IsActive = x.IsActive,
            JobCreatedAt = x.JobCreatedAt,
            JobId = x.JobId,
            JobLastUpdatedAt = x.JobLastUpdatedAt,
            JobSite = x.JobSite,
            OrganizationId = x.OrganizationId,
            RequiredSkills = x.RequiredSkills,
            Timestamp = x.Timestamp
        });
        return Result<IEnumerable<JobFeedResponse>>.Success(response);
    }
}