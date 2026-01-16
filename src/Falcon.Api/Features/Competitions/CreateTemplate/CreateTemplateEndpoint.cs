using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.CreateTemplate;

/// <summary>
/// Endpoint for creating a competition template.
/// </summary>
public class CreateTemplateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/Competition/template",
                [Authorize(Roles = "Teacher,Admin")]
                async (IMediator mediator, [FromBody] CreateTemplateCommand command) =>
                {
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("CreateTemplate")
            .WithTags("Competitions")
            .WithSummary("Create a competition template.")
            .WithDescription(
                "Creates a new competition template which can later be promoted to an active competition by a Teacher or Admin."
            )
            .Produces<CreateTemplateResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
