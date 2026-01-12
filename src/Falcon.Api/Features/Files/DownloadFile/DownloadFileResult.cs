namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Result containing file stream for download.
/// </summary>
/// <param name="FileStream">Stream with file contents. Caller is responsible for disposing the stream.</param>
/// <param name="ContentType">MIME content type to use in the response.</param>
/// <param name="FileName">Suggested filename for the client download.</param>
public record DownloadFileResult(
    Stream FileStream,
    string ContentType,
    string FileName
); 
