using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.Outbox;

public sealed class JobOutboxMessagePublisher(IOutboxRepository repository, IMessagePublisher publisher)
{

    public async Task ProcessAsync(
        CancellationToken cancellationToken = default)
    {
        var messages = await repository.GetUnPublishedMessages(cancellationToken);
        foreach (var message in messages)
        {
            try
            {
                var metadata = new Dictionary<string, string>
                {
                    { "routing-key", message.EventType }
                };
                var envelope = new MessageEnvelope(message.Payload, metadata.AsReadOnly());
                await publisher.PublishAsync(
                    envelope,
                    cancellationToken);

                message.PublishedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                message.RetryCount++;

                message.Error = ex.Message;
            }
        }

        await repository.SaveChangesAsync(
            cancellationToken);
    }
}