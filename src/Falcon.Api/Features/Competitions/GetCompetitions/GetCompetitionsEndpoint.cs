using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Falcon.Core.Domain.Competitions;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Endpoint for getting competitions with optional filtering and pagination.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X GET "https://localhost:5001/api/Competition?status=OpenInscriptions&amp;skip=0&amp;take=10" \\
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class GetCompetitionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Competition", async (IMediator mediator, CompetitionStatus? status, int skip = 0, int take = 10) =>
        {
            var query = new GetCompetitionsQuery(status, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetCompetitions")
        .WithTags("Competitions")
        .Produces<GetCompetitionsResult>();
    }
}
