using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

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
        .Produces<GetStatisticsResult>();
    }
}
