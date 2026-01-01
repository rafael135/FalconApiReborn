using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Endpoint for promoting a competition template to an active competition.
/// </summary>
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
        .Produces<PromoteTemplateResult>();
    }
}
