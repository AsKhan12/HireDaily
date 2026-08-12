using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Jobs.Domain.Abstraction;

namespace Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;

public class UpdateJobHourlyRateCommandHandler(IJobRepository jobRepository) : ICommandHandler<UpdateJobHourlyRateCommand>
{
    public async Task<IResult> Handle(UpdateJobHourlyRateCommand command, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetByIdAsync(command.JobId, cancellationToken);
        if (job is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        job.UpdateHourlyRate(new Money(command.Amount, command.Currency));
        await jobRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
