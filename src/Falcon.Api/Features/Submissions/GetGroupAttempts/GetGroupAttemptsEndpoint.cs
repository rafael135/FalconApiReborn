using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

public class GetGroupAttemptsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Submission/group", [Authorize] async (
            IMediator mediator,
            Guid? competitionId,
            Guid? exerciseId) =>
        {
            var query = new GetGroupAttemptsQuery(competitionId, exerciseId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetGroupAttempts")
        .WithTags("Submissions")
        .Produces<GetGroupAttemptsResult>();
    }
}
