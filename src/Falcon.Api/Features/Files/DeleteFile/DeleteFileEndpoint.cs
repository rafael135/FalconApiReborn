using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Endpoint for deleting files (Admin only).
/// </summary>
public class DeleteFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/File/{id:guid}", [Authorize(Roles = "Admin")] async (IMediator mediator, Guid id) =>
        {
            var command = new DeleteFileCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("DeleteFile")
        .WithTags("Files")
        .Produces<DeleteFileResult>();
    }
}
