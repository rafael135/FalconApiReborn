using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Endpoint for finishing a competition. Only Teacher or Admin can finish a competition.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/finish" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class FinishCompetitionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{id}/finish", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid id) =>
        {
            var command = new FinishCompetitionCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("FinishCompetition")
        .WithTags("Competitions")
        .WithSummary("Finish a competition.")
        .WithDescription("Marks the competition as finished and finalizes rankings; requires Teacher or Admin role.")
        .Produces<FinishCompetitionResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
