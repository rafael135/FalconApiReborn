using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.RemoveExercise;

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
