using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Endpoint for getting competition ranking.
/// </summary>
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
        .Produces<GetRankingResult>();
    }
}
