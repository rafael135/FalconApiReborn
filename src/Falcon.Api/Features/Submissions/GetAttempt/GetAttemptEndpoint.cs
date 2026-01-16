using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.GetAttempt;

/// <summary>
/// Endpoint for retrieving details of a single submission attempt.
/// </summary>
/// <remarks>
/// Returns attempt details including code, language, execution time, and judge response.
/// Only teachers/admins or members of the owning group can access the attempt.
/// Example response: { "attempt": { "id": "...", "exerciseTitle": "Sum", "accepted": false } }
/// </remarks>
public class GetAttemptEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/Submission/{id}",
                [Authorize]
                async (IMediator mediator, Guid id) =>
                {
                    var query = new GetAttemptQuery(id);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetAttempt")
            .WithTags("Submissions")
            .WithSummary("Get attempt details.")
            .WithDescription(
                "Returns details for a specific attempt. Requires authentication and appropriate authorization."
            )
            .Produces<GetAttemptResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
