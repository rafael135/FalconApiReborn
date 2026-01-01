using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Endpoint for uploading files (Teacher/Admin only).
/// </summary>
public static class UploadFileEndpoint
{
    public static IEndpointRouteBuilder MapUploadFile(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/File", [Authorize(Roles = "Teacher,Admin")] async (
            IMediator mediator,
            IFormFile file) =>
        {
            var command = new UploadFileCommand(file);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UploadFile")
        .WithTags("Files")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<UploadFileResult>()
        .DisableAntiforgery(); // Disable antiforgery for file upload

        return app;
    }
}
