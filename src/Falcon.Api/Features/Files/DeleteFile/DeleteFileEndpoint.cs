using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Endpoint for deleting files (Admin only).
/// </summary>
/// <remarks>
/// Deletes a file by Id. Will return 200 with a success message when deletion completes.
/// Example: DELETE /api/File/{id}
/// </remarks>
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
        .WithSummary("Delete a file.")
        .WithDescription("Deletes a file by ID. Requires Admin role.")
        .Produces<DeleteFileResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
} 
