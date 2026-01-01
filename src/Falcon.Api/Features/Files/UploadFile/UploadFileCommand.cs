using MediatR;

namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Command to upload a file.
/// </summary>
public record UploadFileCommand(
    IFormFile File
) : IRequest<UploadFileResult>;
