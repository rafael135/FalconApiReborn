using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Endpoint for getting competition ranking.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X GET "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/ranking" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class GetRankingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Competition/{id}/ranking", async (IMediator mediator, Guid id) =>
        {
            var query = new GetRankingQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetRanking")
        .WithTags("Competitions")
        .WithSummary("Get ranking for a competition.")
        .WithDescription("Returns the ranking table (groups and scores) for the specified competition.")
        .Produces<GetRankingResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

    }
} 
