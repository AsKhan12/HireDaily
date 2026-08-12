using Hiredaily.BuildingBlock.Domain.Entity;

namespace Hiredaily.BuildingBlock.Domain.EntityIds;

public record JobId(Guid Value): IIdentity;