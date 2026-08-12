using Hiredaily.BuildingBlock.Domain.EntityIds;

namespace Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;

public class SuspendOrganizationInput
{
    public required string RequestedBy { get; init; }
    public required OrganizationId OrganizationId { get; init; }
}