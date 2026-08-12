using System.Text.Json;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public class JobHourlyRateUpdatedMessageHandler(IJobFeedRepository _repository)
    : IIntegrationEventHandler
{
    public async Task HandleAsync(string Payload, CancellationToken cancellationToken = default)
    {
        var evt =
            JsonSerializer.Deserialize<JobHourlyRateUpdatedMessage>(Payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true})
                ?? throw new InvalidDataException();

        await _repository.UpdateHourlyRate(evt.JobId, DateTime.UtcNow, evt.HourlyRate, cancellationToken);
    }
}
