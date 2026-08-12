using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Jobs.Application.Shared;
using Hiredaily.Modules.Jobs.Domain.Abstraction;

namespace Hiredaily.Modules.Jobs.Application.GetJob;

public class GetJobRequestHandler(IJobRepository jobRepository) : IRequestHandler<GetJobRequest, GetJobResponse>
{
    public async Task<IResult<GetJobResponse>> Handle(GetJobRequest request, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetByIdAsync(request.JobId, cancellationToken);
        if (job is null)
            return Result<GetJobResponse>.Failure(ValidationResult.Valid(), "not found!");

        return Result<GetJobResponse>.Success(new GetJobResponse
        {
            JobId = job.Id,
            OrganizationId = job.OrganizationId,
            HourlyRate = job.HourlyRate,
            JobSite = job.JobSite,
            RequiredSkills = job.RequiredSkills
                .Select(skill => new SkillDto
                {
                    Name = skill.Name,
                    Field = skill.Field,
                    Description = skill.Description,
                    SkillLevel = skill.SkillLevel
                })
                .ToArray(),
            CreatedAt = job.CreatedAt,
            LastUpdateAt = job.UpdatedAt
        });
    }
}
