using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Endpoint for promoting a competition template to an active competition. Requires Teacher or Admin role.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/promote" \
///   -H "Authorization: Bearer {token}" \
///   -H "Content-Type: application/json" \
///   -d '{ "TemplateId": "00000000-0000-0000-0000-000000000000", "MaxMembers": 3, "MaxExercises": 10, "MaxSubmissionSize": 1024, "Duration": "01:00:00", "StopRanking": "00:45:00", "BlockSubmissions": "00:10:00", "Penalty": "00:05:00" }'
/// </code>
/// </remarks>
public class PromoteTemplateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{id}/promote", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid id, [FromBody] PromoteTemplateCommand command) =>
        {
            if (command.TemplateId != id) return Results.BadRequest(new { error = "Route ID does not match command TemplateId" });
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("PromoteTemplate")
        .WithTags("Competitions")
        .WithSummary("Promote a competition template to active competition.")
        .WithDescription("Promotes the provided template into an active competition; only Teacher or Admin can perform this action.")
        .Produces<PromoteTemplateResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
