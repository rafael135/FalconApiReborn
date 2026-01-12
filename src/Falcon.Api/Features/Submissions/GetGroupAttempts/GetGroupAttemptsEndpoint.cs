using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

/// <summary>
/// Endpoint for retrieving all submissions of the caller's group with optional filtering.
/// </summary>
/// <remarks>
/// Filtra por `competitionId` e/ou `exerciseId` quando fornecidos. Retorna lista de tentativas do grupo do usuário.
/// Exemplo de resposta: { "attempts": [ { "id": "...", "exerciseTitle": "Soma", "accepted": false } ] }
/// Example curl:
/// <code>
/// curl -H "Authorization: Bearer &lt;token&gt;" "https://api.example.com/api/Submission/group?competitionId=00000000-0000-0000-0000-000000000000"
/// </code>
/// </remarks>
public class GetGroupAttemptsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Submission/group", [Authorize] async (
            IMediator mediator,
            Guid? competitionId,
            Guid? exerciseId) =>
        {
            var query = new GetGroupAttemptsQuery(competitionId, exerciseId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetGroupAttempts")
        .WithTags("Submissions")
        .WithSummary("List group attempts.")
        .WithDescription("Returns attempts submitted by the authenticated user's group. Requires authentication.")
        .Produces<GetGroupAttemptsResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
} 
