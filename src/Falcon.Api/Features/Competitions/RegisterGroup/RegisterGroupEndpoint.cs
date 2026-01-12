using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Endpoint for registering the current user's group in a competition. Requires authentication.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/register" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class RegisterGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{id}/register", [Authorize] async (IMediator mediator, Guid id) =>
        {
            var command = new RegisterGroupCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RegisterGroup")
        .WithTags("Competitions")
        .WithSummary("Register the current user's group in a competition.")
        .WithDescription("Registers the calling user's group into the specified competition; returns registration confirmation or error if not eligible.")
        .Produces<RegisterGroupResult>();
    }
}
