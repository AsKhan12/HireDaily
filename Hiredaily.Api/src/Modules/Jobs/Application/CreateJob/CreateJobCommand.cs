using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.Modules.Jobs.Application.Shared;

namespace Hiredaily.Modules.Jobs.Application.CreateJob;

public class CreateJobCommand : ICommand
{
    public Guid RequestId { get; set; }
    public required string Title { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public decimal HourlyRateAmount { get; set; }
    public string HourlyRateCurrency { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public IReadOnlyList<SkillDto> RequiredSkills { get; set; } = [];
}
