using Falcon.Api.Extensions;
using Falcon.Api.Features.Questions.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Questions.GetQuestion;

/// <summary>
/// Endpoint for retrieving a single question by ID.
/// </summary>
public class GetQuestionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Question/{id:guid}", [Authorize] async (
            IMediator mediator,
            Guid id) =>
        {
            var query = new GetQuestionQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetQuestion")
        .WithTags("Questions")
        .Produces<QuestionDto>()
        .Produces(401)
        .Produces(404);
    }
}
