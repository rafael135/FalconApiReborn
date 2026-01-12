using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Admin.GenerateTeacherToken;

/// <summary>
/// Endpoint for generating teacher registration tokens (Admin only).
/// </summary>
public class GenerateTeacherTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/Admin/teacher-token", [Authorize(Roles = "Admin")] async (
            IMediator mediator,
            int expirationHours = 168) =>
        {
            var command = new GenerateTeacherTokenCommand(expirationHours);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("GenerateTeacherToken")
        .WithTags("Admin")
        .WithSummary("Generate an access token for teacher registration.")
        .WithDescription("Generates a single-use token that can be used to register a Teacher account. Requires Admin role.")
        .Produces<GenerateTeacherTokenResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}
