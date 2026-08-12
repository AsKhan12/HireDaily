namespace Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;

public class RegisterOrganizationInput
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}