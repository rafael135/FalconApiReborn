using MediatR;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Command to delete a file.
/// </summary>
public record DeleteFileCommand(
    Guid FileId
) : IRequest<DeleteFileResult>;
