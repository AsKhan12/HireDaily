using Hiredaily.BuildingBlock.Application.Mediator.Pipeline;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.API.Chassis;

public class BehaviorConfiguration : IBehaviorConfiguration
{
    public void Configure(IBehaviorCollection behaviors)
    {
        behaviors.Add<ValidationPipelineBehavior>();
    }
}
