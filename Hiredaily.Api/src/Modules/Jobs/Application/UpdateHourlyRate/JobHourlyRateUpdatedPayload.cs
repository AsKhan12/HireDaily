using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;

public record JobHourlyRateUpdatedPayload(JobId JobId, Money HourlyRate) : IMessagePayload;