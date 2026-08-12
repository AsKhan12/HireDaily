using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Jobs.Domain;
using Hiredaily.Modules.Jobs.Domain.Abstraction;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Domain.EntityIds;

namespace Hiredaily.Modules.Jobs.Application.CreateJob;

public class CreateJobCommandHandler(
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateJobCommand>
{
    public async Task<IResult> Handle(CreateJobCommand command, CancellationToken cancellationToken)
    {
        var job = new Job(
            new Money(command.HourlyRateAmount, command.HourlyRateCurrency),
            new JobSite(
                new GeoLocation(command.Latitude, command.Longitude),
                new PostalAddress(
                    command.AddressLine1,
                    command.AddressLine2,
                    command.City,
                    command.State,
                    command.Country,
                    command.PostalCode)),
            command.RequiredSkills
                .Select(skill => new Skill(skill.Name, skill.Field, skill.Description, skill.SkillLevel))
                .ToArray(),
            new OrganizationId(command.OrganizationId),
            command.Title);

        await jobRepository.AddAsync(job, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
