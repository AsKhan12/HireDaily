using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Domain.EntityIds;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

public record JobTitleMessagePayload(string Title, JobId JobId) : IMessagePayload;