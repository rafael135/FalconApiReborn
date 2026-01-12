using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Endpoint for getting detailed competition information by ID.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X GET "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class GetCompetitionEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to return detailed competition information by id.
    /// </summary>
    /// <param name="app">The route builder to map the endpoint into.</param>
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
        .WithSummary("Get competition details by ID.")
        .WithDescription("Returns detailed information about a specific competition, including exercises and schedule.")
        .Produces<GetCompetitionResult>();
    }
}
