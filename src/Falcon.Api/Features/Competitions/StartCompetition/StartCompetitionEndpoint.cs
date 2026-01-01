using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Endpoint for starting a competition.
/// </summary>
public class StartCompetitionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{id}/start", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid id) =>
        {
            var command = new StartCompetitionCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("StartCompetition")
        .WithTags("Competitions")
        .Produces<StartCompetitionResult>();
    }
}
