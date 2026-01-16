using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Endpoint for starting a competition. Only users in the Teacher or Admin roles are authorized to start a competition.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/start" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class StartCompetitionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/Competition/{id}/start",
                [Authorize(Roles = "Teacher,Admin")]
                async (IMediator mediator, Guid id) =>
                {
                    var command = new StartCompetitionCommand(id);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("StartCompetition")
            .WithTags("Competitions")
            .WithSummary("Start a competition.")
            .WithDescription(
                "Starts the specified competition immediately; requires Teacher or Admin role."
            )
            .Produces<StartCompetitionResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
