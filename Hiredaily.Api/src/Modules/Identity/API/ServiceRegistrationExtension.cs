using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.Modules.Identity.API.Chassis;
using Hiredaily.Modules.Identity.Application.Organizations.RegisterOrganization;
using Hiredaily.Modules.Identity.Application.Organizations.GetOrganization;
using Hiredaily.Modules.Identity.Application.Organizations.ActivateOrganization;
using Hiredaily.Modules.Identity.Application.Organizations.SuspendOrganization;
using Hiredaily.Modules.Identity.Application.Organizations.UpdateOrganization;
using Hiredaily.Modules.Identity.Application.Users.AddUserSkill;
using Hiredaily.Modules.Identity.Application.Users.GetUser;
using Hiredaily.Modules.Identity.Application.Users.RemoveUserSkill;
using Hiredaily.Modules.Identity.Application.Users.RegisterUser;
using Hiredaily.Modules.Identity.Application.Users.UpdateUserName;
using Hiredaily.Modules.Identity.Application.Users.UpdateUserAddress;
using Hiredaily.Modules.Identity.Application.Abstraction;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;
using Hiredaily.Modules.Identity.Infra.Persistence;
using Hiredaily.Modules.Identity.Infra.Persistence.Repostories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hiredaily.BuildingBlock.API;

namespace Hiredaily.Modules.Identity.API;

public static class ServiceRegistrationExtension
{
    public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatr<BehaviorConfiguration>();
        services.AddSingleton<ValidationPipelineBehavior>();
        services.AddScoped<ICommandHandler<RegisterOrganizationCommand>, RegisterOrganizationCommandHandler>();
        services.AddScoped<ICommandHandler<ActivateOrganizationCommand>, ActivateOrganizationCommandHandler>();
        services.AddScoped<ICommandHandler<SuspendOrganizationCommand>, SuspendOrganizationCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateOrganizationCommand>, UpdateOrganizationCommandHandler>();
        services.AddScoped<ICommandHandler<RegisterUserCommand>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserNameCommand>, UpdateUserNameCommandHandler>();
        services.AddScoped<ICommandHandler<AddUserSkillCommand>, AddUserSkillCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveUserSkillCommand>, RemoveUserSkillCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserAddressCommand>, UpdateUserAddressCommandHandler>();
        services.AddScoped<IRequestHandler<GetUserRequest, GetUserResponse>, GetUserRequestHandler>();
        services.AddScoped<IRequestHandler<GetOrganizationRequest, GetOrganizationResponse>, GetOrganizationRequestHandler>();
        services.AddScoped<IValidator<RegisterOrganizationCommand>, RegisterOrganizationCommandValidator>();
        services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();
        services.AddScoped<IValidator<UpdateUserNameCommand>, UpdateUserNameCommandValidator>();
        services.AddScoped<IValidator<AddUserSkillCommand>, AddUserSkillCommandValidator>();
        services.AddScoped<IValidator<RemoveUserSkillCommand>, RemoveUserSkillCommandValidator>();
        services.AddScoped<IValidator<UpdateUserAddressCommand>, UpdateUserAddressCommandValidator>();
        services.AddScoped<IValidator<GetUserRequest>, GetUserRequestValidator>();
        services.AddScoped<IValidator<GetOrganizationRequest>, GetOrganizationRequestValidator>();
        services.AddScoped<IValidator<ActivateOrganizationCommand>, ActivateOrganizationCommandValidator>();
        services.AddScoped<IValidator<SuspendOrganizationCommand>, SuspendOrganizationCommandValidator>();
        services.AddScoped<IValidator<UpdateOrganizationCommand>, UpdateOrganizationCommandValidator>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenStoreRepository, RefreshTokenStoreRepository>();
        services.AddScoped<Security.IJwtTokenService, Security.JwtTokenService>();
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(
            configuration.GetConnectionString("IdentityDbConnection"));
        });
    }
}
