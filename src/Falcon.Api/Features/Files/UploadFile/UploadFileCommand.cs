using MediatR;

namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Command to upload a file.
/// </summary>
/// <param name="File">The file to upload (multipart/form-data).</param>
public record UploadFileCommand(
    IFormFile File
) : IRequest<UploadFileResult>; 
