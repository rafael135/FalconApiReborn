using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Admin.GetStatistics;

/// <summary>
/// Endpoint for getting system statistics (Admin only).
/// </summary>
public class GetStatisticsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Admin/stats", [Authorize(Roles = "Admin")] async (
            IMediator mediator) =>
        {
            var query = new GetStatisticsQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetStatistics")
        .WithTags("Admin")
        .WithSummary("Get system statistics for admin dashboard.")
        .WithDescription("Provides aggregated user, competition, exercise and submission statistics; Admin only.")
        .Produces<GetStatisticsResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}
