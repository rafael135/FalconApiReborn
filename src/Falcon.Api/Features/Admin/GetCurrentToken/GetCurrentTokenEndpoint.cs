using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Admin.GetCurrentToken;

/// <summary>
/// Endpoint for getting current teacher registration token (Admin only).
/// </summary>
public class GetCurrentTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Admin/teacher-token", [Authorize(Roles = "Admin")] async (
            IMediator mediator) =>
        {
            var query = new GetCurrentTokenQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetCurrentToken")
        .WithTags("Admin")
        .Produces<GetCurrentTokenResult>();
    }
}
