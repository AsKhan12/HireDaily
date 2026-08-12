using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Hiredaily.Host.API.Middlewares;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception");

        var problem = exception switch
        {
            ArgumentException => 
                new ProblemDetails 
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request"
                },
            OperationCanceledException => 
                new ProblemDetails
                {
                    Status = StatusCodes.Status499ClientClosedRequest,
                    Title = "Task cancelled"
                },
            _ => 
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error"
                }
        };

        httpContext.Response.StatusCode = problem.Status.GetHashCode();

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}