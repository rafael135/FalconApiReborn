using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.GetExercise;

/// <summary>
/// Endpoint for getting an exercise by ID.
/// </summary>
/// <remarks>
/// Returns detailed exercise information. **Note:** test cases are included only if the caller is
/// in the **Teacher** or **Admin** role — otherwise the `testCases` property will be null.
/// Example:
/// <code>
/// curl "https://localhost:5001/api/Exercise/{exerciseId}"
/// </code>
/// </remarks>
public class GetExerciseEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/Exercise/{id}",
                async (IMediator mediator, Guid id) =>
                {
                    var query = new GetExerciseQuery(id);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetExercise")
            .WithTags("Exercises")
            .WithSummary("Get an exercise by ID.")
            .WithDescription(
                "Returns exercise details. Test cases are included only for Teacher/Admin; otherwise testCases is null."
            )
            .Produces<GetExerciseResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
