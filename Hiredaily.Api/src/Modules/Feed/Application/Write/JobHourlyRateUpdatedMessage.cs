using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public record JobHourlyRateUpdatedMessage(JobId JobId, Money HourlyRate); 
