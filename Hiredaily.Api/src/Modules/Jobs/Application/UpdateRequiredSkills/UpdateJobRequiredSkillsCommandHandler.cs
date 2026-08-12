using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Jobs.Domain.Abstraction;

namespace Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;

public class UpdateJobRequiredSkillsCommandHandler(IJobRepository jobRepository) : ICommandHandler<UpdateJobRequiredSkillsCommand>
{
    public async Task<IResult> Handle(UpdateJobRequiredSkillsCommand command, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetByIdAsync(command.JobId, cancellationToken);
        if (job is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        job.UpdateRequiredSkills(command.RequiredSkills
            .Select(skill => new Skill(skill.Name, skill.Field, skill.Description, skill.SkillLevel))
            .ToArray());

        await jobRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
