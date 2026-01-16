using MediatR;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Command to delete a file.
/// </summary>
/// <param name="FileId">Identifier of the file to delete.</param>
public record DeleteFileCommand(Guid FileId) : IRequest<DeleteFileResult>;
