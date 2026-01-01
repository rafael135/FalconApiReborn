using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Admin.GenerateTeacherToken;

/// <summary>
/// Endpoint for generating teacher registration tokens (Admin only).
/// </summary>
public static class GenerateTeacherTokenEndpoint
{
    public static IEndpointRouteBuilder MapGenerateTeacherToken(this IEndpointRouteBuilder app)
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
        .Produces<GenerateTeacherTokenResult>();

        return app;
    }
}
