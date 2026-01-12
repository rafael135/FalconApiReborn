using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.AddTestCase;

/// <summary>
/// Endpoint para adicionar um caso de teste a um exercício.
/// </summary>
/// <remarks>
/// Exemplo de uso:
/// <code>
/// curl -X POST "https://localhost:5001/api/Exercise/{exerciseId}/testcase" \
///  -H "Content-Type: application/json" \
///  -d '{ "exerciseId": "00000000-0000-0000-0000-000000000000", "inputContent": "1 2", "expectedOutput": "3" }'
/// </code>
/// </remarks>
public class AddTestCaseEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Exercise/{id}/testcase", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid id, [FromBody] AddTestCaseCommand command) =>
        {
            if (command.ExerciseId != id) return Results.BadRequest(new { error = "Route ID does not match command ExerciseId" });
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("AddTestCase")
        .WithTags("Exercises")
        .Produces<AddTestCaseResult>();
    }
} 
