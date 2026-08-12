using Hiredaily.BuildingBlock.Domain.Entity;

namespace Hiredaily.BuildingBlock.Domain.EntityIds;

public record UserId(Guid Value): IIdentity;
