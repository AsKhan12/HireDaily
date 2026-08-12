using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Application.Users.Shared;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Users.GetUser;

public class GetUserRequestHandler(IUserRepository userRepository) : IRequestHandler<GetUserRequest, GetUserResponse>
{
    public async Task<IResult<GetUserResponse>> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<GetUserResponse>.Failure(ValidationResult.Valid(), "not found!");

        return Result<GetUserResponse>.Success(new GetUserResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Username = user.Username,
            Address = user.Address,
            Skills = user.Skills
                .Select(skill => new SkillDto
                {
                    Name = skill.Name,
                    Field = skill.Field,
                    Description = skill.Description,
                    SkillLevel = skill.SkillLevel
                })
                .ToArray(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }
}
