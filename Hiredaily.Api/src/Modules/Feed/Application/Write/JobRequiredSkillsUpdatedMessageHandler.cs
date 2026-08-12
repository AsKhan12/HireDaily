using System.Text.Json;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public class JobRequiredSkillsUpdatedMessageHandler(IJobFeedRepository _repository)
    : IIntegrationEventHandler
{
    public async Task HandleAsync(string Payload, CancellationToken cancellationToken = default)
    {
        var evt =
            JsonSerializer.Deserialize<JobRequiredSkillsUpdatedMessage>(
                Payload,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException();

        await _repository.UpdateRequiredSkills(evt.JobId, DateTime.UtcNow, evt.RequiredSkills, cancellationToken);
    }
}
