using Falcon.Core.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        ProblemDetails problemDetails = new ProblemDetails { Instance = httpContext.Request.Path };

        switch (exception)
        {
            case FormException formException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = formException.Message;
                problemDetails.Extensions["errors"] = formException.Errors;
                break;

            case NotFoundException notFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = notFoundException.Message;
                break;

            case BusinessRuleException businessRuleException:
                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "Business Rule Violation";
                problemDetails.Detail = businessRuleException.Message;
                break;

            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
