using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

/// <summary>
/// Endpoint para remover um caso de teste de um exercício.
/// </summary>
/// <remarks>
/// Exemplo usando curl:
/// <code>
/// curl -X DELETE "https://localhost:5001/api/Exercise/{exerciseId}/testcase/{testCaseId}"
/// </code>
/// </remarks>
public class RemoveTestCaseEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Exercise/{exerciseId}/testcase/{testCaseId}", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid exerciseId, Guid testCaseId) =>
        {
            var command = new RemoveTestCaseCommand(exerciseId, testCaseId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RemoveTestCase")
        .WithTags("Exercises")
        .Produces<RemoveTestCaseResult>();
    }
} 
