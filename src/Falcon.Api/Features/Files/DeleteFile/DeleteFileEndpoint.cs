using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Endpoint for deleting files (Admin only).
/// </summary>
public static class DeleteFileEndpoint
{
    public static IEndpointRouteBuilder MapDeleteFile(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/File/{id:guid}", [Authorize(Roles = "Admin")] async (
            IMediator mediator,
            Guid id) =>
        {
            var command = new DeleteFileCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("DeleteFile")
        .WithTags("Files")
        .Produces<DeleteFileResult>();

        return app;
    }
}
