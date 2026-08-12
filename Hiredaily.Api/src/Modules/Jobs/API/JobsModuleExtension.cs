using Hiredaily.Modules.Jobs.API.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace Hiredaily.Modules.Jobs.API;

public static class JobsModuleExtension
{
    public static void UseJobsModule(this WebApplication app)
    {
        app.MapJobEndpoints();
    }
}
