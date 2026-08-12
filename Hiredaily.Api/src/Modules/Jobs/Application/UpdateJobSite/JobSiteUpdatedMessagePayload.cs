using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobSite;

public record JobSiteUpdatedMessagePayload(JobId JobId, JobSite JobSite) : IMessagePayload;