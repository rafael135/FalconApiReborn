using MediatR;

namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Query to download a file.
/// </summary>
public record DownloadFileQuery(
    Guid FileId
) : IRequest<DownloadFileResult>;
