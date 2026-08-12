using System.Text.Json;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public class JobSiteUpdatedMessageHandler(IJobFeedRepository _repository)
    : IIntegrationEventHandler
{
    public async Task HandleAsync(string Payload, CancellationToken cancellationToken = default)
    {
        var evt =
            JsonSerializer.Deserialize<JobSiteUpdatedMessage>(
                Payload,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException();

        await _repository.UpdateJobSite(evt.JobId, DateTime.UtcNow, evt.JobSite, cancellationToken);
    }
}
