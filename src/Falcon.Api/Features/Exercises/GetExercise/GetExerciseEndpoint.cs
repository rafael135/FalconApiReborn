using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.GetExercise;

/// <summary>
/// Endpoint for getting an exercise by ID.
/// </summary>
public class GetExerciseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Exercise/{id}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetExerciseQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetExercise")
        .WithTags("Exercises")
        .Produces<GetExerciseResult>();
    }
}
