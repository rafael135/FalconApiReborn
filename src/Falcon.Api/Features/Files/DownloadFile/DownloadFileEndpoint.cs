using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Endpoint for downloading files (authenticated users).
/// </summary>
/// <remarks>
/// Returns the raw file bytes using the content type stored with the file. Example: GET /api/File/{id}
/// Example curl: curl -H "Authorization: Bearer &lt;token&gt;" https://api.example.com/api/File/00000000-0000-0000-0000-000000000000 --output file.bin
/// </remarks>
public class DownloadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/File/{id:guid}", [Authorize] async (IMediator mediator, Guid id) =>
        {
            var query = new DownloadFileQuery(id);
            var result = await mediator.Send(query);
            
            return Results.File(result.FileStream, result.ContentType, result.FileName);
        })
        .WithName("DownloadFile")
        .WithTags("Files")
        .Produces(200, contentType: "application/octet-stream");
    }
} 
