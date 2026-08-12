using Hiredaily.Modules.Jobs.Domain.Abstraction;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

public class UpdateJobTitleCommandHandler(IJobRepository jobRepository) : ICommandHandler<UpdateJobTitleCommand>
{
    public async Task<IResult> Handle(UpdateJobTitleCommand command, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetByIdAsync(command.JobId, cancellationToken);
        if (job is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        job.UpdateTitle(command.Title);

        await jobRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}