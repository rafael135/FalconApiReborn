using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

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
        app.MapPost("api/File", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, IFormFile file) =>
        {
            var command = new UploadFileCommand(file);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UploadFile")
        .WithTags("Files")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<UploadFileResult>()
        .DisableAntiforgery();
    }
} 
