using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Jobs.Domain.Abstraction;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobSite;

public class UpdateJobSiteCommandHandler(IJobRepository jobRepository) : ICommandHandler<UpdateJobSiteCommand>
{
    public async Task<IResult> Handle(UpdateJobSiteCommand command, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetByIdAsync(command.JobId, cancellationToken);
        if (job is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        job.UpdateJobSite(new JobSite(
            new GeoLocation(command.Latitude, command.Longitude),
            new PostalAddress(
                command.AddressLine1,
                command.AddressLine2,
                command.City,
                command.State,
                command.Country,
                command.PostalCode)));

        await jobRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

