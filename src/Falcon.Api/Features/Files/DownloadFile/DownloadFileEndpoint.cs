using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Endpoint for downloading files (authenticated users).
/// </summary>
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
