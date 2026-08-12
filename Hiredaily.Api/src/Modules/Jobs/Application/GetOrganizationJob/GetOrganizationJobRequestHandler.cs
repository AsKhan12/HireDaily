using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Jobs.Application.Shared;
using Hiredaily.Modules.Jobs.Domain.Abstraction;

namespace Hiredaily.Modules.Jobs.Application.GetOrganizationJob;

public class GetOrganizationJobRequestHandler(IJobRepository jobRepository) : IRequestHandler<GetOrganizatoinJobRequest, GetOrganizationJobResponse>
{
    public async Task<IResult<GetOrganizationJobResponse>> Handle(GetOrganizatoinJobRequest request, CancellationToken cancellationToken)
    {
        var jobs = await jobRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);
        if (jobs is null)
            return Result<GetOrganizationJobResponse>.Failure(ValidationResult.Valid(), "not found!");

        return Result<GetOrganizationJobResponse>.Success(new GetOrganizationJobResponse
        {
            Jobs = [.. jobs.Select(job => new JobsDto
            {
                JobId = job.Id,
                OrganizationId = job.OrganizationId,
                HourlyRate = job.HourlyRate,
                JobSite = job.JobSite,
                RequiredSkills = [.. job.RequiredSkills
                .Select(skill => new SkillDto
                {
                    Name = skill.Name,
                    Field = skill.Field,
                    Description = skill.Description,
                    SkillLevel = skill.SkillLevel
                })],
                CreatedAt = job.CreatedAt,
                LastUpdateAt = job.UpdatedAt
            })]
        });
    }
}
