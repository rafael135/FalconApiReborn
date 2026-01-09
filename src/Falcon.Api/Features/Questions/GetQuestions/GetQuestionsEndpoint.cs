using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Questions.GetQuestions;

/// <summary>
/// Endpoint for retrieving paginated questions.
/// </summary>
public class GetQuestionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Question", [Authorize] async (
            IMediator mediator,
            [FromQuery] Guid competitionId,
            [FromQuery] Guid? exerciseId = null,
            [FromQuery] int? questionType = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10) =>
        {
            var query = new GetQuestionsQuery(
                competitionId, 
                exerciseId, 
                questionType, 
                skip, 
                take
            );
            
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetQuestions")
        .WithTags("Questions")
        .Produces<GetQuestionsResult>()
        .Produces(401);
    }
}
