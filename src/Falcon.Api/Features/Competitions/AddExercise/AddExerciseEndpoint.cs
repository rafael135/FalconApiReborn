using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.AddExercise;

public class AddExerciseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{competitionId}/exercise/{exerciseId}", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid competitionId, Guid exerciseId) =>
        {
            var command = new AddExerciseCommand(competitionId, exerciseId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("AddExercise")
        .WithTags("Competitions")
        .Produces<AddExerciseResult>();
    }
}
