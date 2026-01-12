using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Submissions.GetAttempt;

/// <summary>
/// Endpoint for retrieving details of a single submission attempt.
/// </summary>
/// <remarks>
/// Retorna detalhes da tentativa, incluindo código, linguagem, tempo de execução e resposta do juiz.
/// Somente professores/admins ou membros do grupo podem acessar a tentativa.
/// Exemplo de resposta: { "attempt": { "id": "...", "exerciseTitle": "Soma", "accepted": false } }
/// </remarks>
public class GetAttemptEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Submission/{id}", [Authorize] async (
            IMediator mediator,
            Guid id) =>
        {
            var query = new GetAttemptQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAttempt")
        .WithTags("Submissions")
        .WithSummary("Get attempt details.")
        .WithDescription("Returns details for a specific attempt. Requires authentication and appropriate authorization.")
        .Produces<GetAttemptResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
} 
