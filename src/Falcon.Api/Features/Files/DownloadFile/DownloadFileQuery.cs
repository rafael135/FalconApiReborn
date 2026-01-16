using MediatR;

namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Query to download a file.
/// </summary>
/// <param name="FileId">Identifier of the file to download.</param>
public record DownloadFileQuery(Guid FileId) : IRequest<DownloadFileResult>;
