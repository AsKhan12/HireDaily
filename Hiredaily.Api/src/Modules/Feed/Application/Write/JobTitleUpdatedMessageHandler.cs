using System.Text.Json;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public class JobTitleUpdatedMessageHandler(IJobFeedRepository _repository)
    : IIntegrationEventHandler
{
    public async Task HandleAsync(string Payload, CancellationToken cancellationToken = default)
    {
        JobTitleUpdatedMessage evt = 
                JsonSerializer.Deserialize<JobTitleUpdatedMessage>( Payload , new JsonSerializerOptions { PropertyNameCaseInsensitive = true})
                    ?? throw new InvalidDataException();

        await _repository.UpdateTitle(evt.JobId, evt.Title, DateTime.UtcNow, cancellationToken);
    }
}
