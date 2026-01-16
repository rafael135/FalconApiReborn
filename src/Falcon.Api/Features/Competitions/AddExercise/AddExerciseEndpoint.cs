using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.AddExercise;

/// <summary>
/// Endpoint to add an existing exercise to a competition. Requires Teacher or Admin role.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/exercise/00000000-0000-0000-0000-000000000000" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class AddExerciseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/Competition/{competitionId}/exercise/{exerciseId}",
                [Authorize(Roles = "Teacher,Admin")]
                async (IMediator mediator, Guid competitionId, Guid exerciseId) =>
                {
                    var command = new AddExerciseCommand(competitionId, exerciseId);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("AddExercise")
            .WithTags("Competitions")
            .WithSummary("Add an exercise to a competition.")
            .WithDescription(
                "Adds an existing exercise to the specified competition. Requires Teacher or Admin role."
            )
            .Produces<AddExerciseResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
