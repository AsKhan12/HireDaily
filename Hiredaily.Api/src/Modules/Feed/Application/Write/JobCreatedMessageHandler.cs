using System.Text.Json;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public class JobCreatedMessageHandler(IJobFeedRepository _repository)
    : IIntegrationEventHandler
{
    public async Task HandleAsync(string Payload, CancellationToken cancellationToken = default)
    {
        JobCreatedMessage evt = 
                JsonSerializer.Deserialize<JobCreatedMessage>( Payload , new JsonSerializerOptions { PropertyNameCaseInsensitive = true})
                    ?? throw new InvalidDataException();
        var feed = JobFeed.Create(
            DateTime.UtcNow,
            evt.JobId,
            evt.Title,
            evt.JobCreatedAt,
            evt.JobLastUpdatedAt,
            evt.OrganizationId,
            evt.HourlyRate,
            evt.JobSite,
            evt.RequiredSkills);

        await _repository.Insert(feed, cancellationToken);
    }
}
