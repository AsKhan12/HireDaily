using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

public class UpdateJobTitleCommand : ICommand
{
    public Guid RequestId {get; init;} = Guid.NewGuid();

    public DateTime RequestedAt {get; init;} = DateTime.UtcNow;

    public required string RequestedBy {get; init;}

    public required string Title {get; init;}
    public required JobId JobId {get; init;}
}