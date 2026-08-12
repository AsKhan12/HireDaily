using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;

public class UpdateJobHourlyRateCommand : ICommand
{
    public Guid RequestId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public JobId JobId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
