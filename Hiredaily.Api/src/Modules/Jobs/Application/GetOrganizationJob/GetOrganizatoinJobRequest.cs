using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Jobs.Application.GetOrganizationJob;

public class GetOrganizatoinJobRequest : IRequest<GetOrganizationJobResponse>
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string RequestedBy { get; set; } = string.Empty;
    public OrganizationId OrganizationId { get; set; } = default!;
}
