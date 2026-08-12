using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Jobs.Application.GetJob;

public class GetJobRequest : IRequest<GetJobResponse>
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string RequestedBy { get; set; } = string.Empty;
    public JobId JobId { get; set; } = default!;
}
