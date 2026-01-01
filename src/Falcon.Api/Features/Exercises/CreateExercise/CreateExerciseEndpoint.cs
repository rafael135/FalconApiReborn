using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Endpoint for creating a new exercise.
/// </summary>
public class CreateExerciseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Exercise", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, [FromBody] CreateExerciseCommand command) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateExercise")
        .WithTags("Exercises")
        .Produces<CreateExerciseResult>();
    }
}
