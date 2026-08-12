using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Identity.Application.Organizations.GetOrganization;

public class GetOrganizationRequest : IRequest<GetOrganizationResponse>
{
    public Guid RequestId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; }
    public OrganizationId OrganizationId { get; set; }
}