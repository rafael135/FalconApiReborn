using Falcon.Api.Extensions;
using Falcon.Api.Features.Questions.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Questions.GetQuestion;

/// <summary>
/// Endpoint for retrieving a single question by ID.
/// </summary>
public class GetQuestionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/Question/{id:guid}",
                [Authorize]
                async (IMediator mediator, Guid id) =>
                {
                    var query = new GetQuestionQuery(id);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetQuestion")
            .WithTags("Questions")
            .WithSummary("Get question details.")
            .WithDescription(
                "Returns detailed information for a single question. Requires authentication when the competition is protected."
            )
            .Produces<QuestionDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }
}
