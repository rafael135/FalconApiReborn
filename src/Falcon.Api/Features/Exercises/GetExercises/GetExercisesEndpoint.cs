using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.GetExercises;

/// <summary>
/// Endpoint for getting exercises with optional filtering.
/// </summary>
public class GetExercisesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Exercise", async (IMediator mediator, int? exerciseTypeId = null, int skip = 0, int take = 10) =>
        {
            var query = new GetExercisesQuery(exerciseTypeId, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetExercises")
        .WithTags("Exercises")
        .Produces<GetExercisesResult>();
    }
}
