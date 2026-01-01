using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Endpoint for getting detailed competition information.
/// </summary>
public class GetCompetitionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Competition/{id}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetCompetitionQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetCompetition")
        .WithTags("Competitions")
        .Produces<GetCompetitionResult>();
    }
}
