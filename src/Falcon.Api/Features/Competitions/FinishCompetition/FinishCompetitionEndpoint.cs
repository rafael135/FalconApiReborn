using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Endpoint for finishing a competition.
/// </summary>
public class FinishCompetitionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{id}/finish", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid id) =>
        {
            var command = new FinishCompetitionCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("FinishCompetition")
        .WithTags("Competitions")
        .Produces<FinishCompetitionResult>();
    }
}
