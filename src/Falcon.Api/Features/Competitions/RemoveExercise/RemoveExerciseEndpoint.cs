using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.RemoveExercise;

/// <summary>
/// Endpoint to remove an exercise from a competition. Requires Teacher or Admin role.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X DELETE "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/exercise/00000000-0000-0000-0000-000000000000" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class RemoveExerciseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Competition/{competitionId}/exercise/{exerciseId}", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid competitionId, Guid exerciseId) =>
        {
            var command = new RemoveExerciseCommand(competitionId, exerciseId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RemoveExercise")
        .WithTags("Competitions")
        .Produces<RemoveExerciseResult>();
    }
} 
