using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Endpoint for uploading files (Teacher/Admin only).
/// </summary>
/// <remarks>
/// Accepts multipart/form-data with a single file field named "file".
/// Supported extensions: .pdf, .zip, .txt, .md, .jpg, .png. Maximum size: 10 MB.
/// Example curl:
/// <code>
/// curl -F "file=@exercise.pdf" -H "Authorization: Bearer &lt;token&gt;" https://api.example.com/api/File
/// </code>
/// </remarks>
public class UploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/File",
                [Authorize(Roles = "Teacher,Admin")]
                async (IMediator mediator, IFormFile file) =>
                {
                    var command = new UploadFileCommand(file);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("UploadFile")
            .WithTags("Files")
            .WithSummary("Upload an attached file.")
            .WithDescription(
                "Upload a file via multipart/form-data using the 'file' field. Supported types: .pdf, .zip, .txt, .md, .jpg, .png. Max 10MB."
            )
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadFileResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .DisableAntiforgery();
    }
}
