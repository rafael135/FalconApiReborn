namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Result containing file stream for download.
/// </summary>
public record DownloadFileResult(
    Stream FileStream,
    string ContentType,
    string FileName
);
