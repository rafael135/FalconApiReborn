using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Logs.GetLogs;

/// <summary>
/// Endpoint for getting system logs (Teacher/Admin only).
/// </summary>
public class GetLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/Log",
                [Authorize(Roles = "Teacher,Admin")]
                async (
                    IMediator mediator,
                    string? userId,
                    Guid? groupId,
                    Guid? competitionId,
                    int? actionType,
                    DateTime? startDate,
                    DateTime? endDate,
                    int skip = 0,
                    int take = 50
                ) =>
                {
                    var query = new GetLogsQuery(
                        userId,
                        groupId,
                        competitionId,
                        actionType,
                        startDate,
                        endDate,
                        skip,
                        take
                    );
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetLogs")
            .WithTags("Logs")
            .WithSummary("Get system logs.")
            .WithDescription(
                "Returns system logs filtered by provided query parameters. Requires Teacher or Admin role."
            )
            .Produces<GetLogsResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
