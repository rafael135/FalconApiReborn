using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.CreateTemplate;

/// <summary>
/// Endpoint for creating a competition template.
/// </summary>
public class CreateTemplateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/template", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, [FromBody] CreateTemplateCommand command) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateTemplate")
        .WithTags("Competitions")
        .Produces<CreateTemplateResult>();
    }
}
