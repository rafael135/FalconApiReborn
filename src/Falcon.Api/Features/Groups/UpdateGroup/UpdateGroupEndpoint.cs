using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.UpdateGroup;

/// <summary>
/// Endpoint for updating a group's name.
/// </summary>
public class UpdateGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/Group/{id}", [Authorize] async (IMediator mediator, Guid id, [FromBody] UpdateGroupCommand command) =>
        {
            if (id != command.GroupId) return Results.BadRequest("O ID da rota não corresponde ao ID do comando");
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateGroup")
        .WithTags("Groups")
        .Produces<UpdateGroupResult>();
    }
}
