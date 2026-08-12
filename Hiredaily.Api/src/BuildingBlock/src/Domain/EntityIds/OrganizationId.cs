using Hiredaily.BuildingBlock.Domain.Entity;

namespace Hiredaily.BuildingBlock.Domain.EntityIds;

public record OrganizationId(Guid Value): IIdentity;
