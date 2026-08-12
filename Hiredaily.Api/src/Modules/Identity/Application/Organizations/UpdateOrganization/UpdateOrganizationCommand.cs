using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;
namespace Hiredaily.Modules.Identity.Application.Organizations.UpdateOrganization;

public class UpdateOrganizationCommand : ICommand
{
    public Guid RequestId { get; set; }
    public OrganizationId OrganizationId { get; set;}
    public DateTime RequestedAt { get; set; }
    public string? RequestedBy { get; set; }
    public string? UpdatedName {get; set;}
    public string? UpdatedDescription { get; set; }
    public OrganizationAddress? UpdatedAddress {get; set;}
}
