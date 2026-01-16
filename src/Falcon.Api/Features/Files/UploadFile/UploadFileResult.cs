namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Result of file upload operation.
/// </summary>
/// <param name="FileId">Identifier of the newly created attached file.</param>
/// <param name="FileName">Original filename.</param>
/// <param name="FileType">MIME type of the file.</param>
/// <param name="FileSize">Size in bytes.</param>
/// <param name="CreatedAt">Timestamp when the file record was created (UTC).</param>
public record UploadFileResult(
    Guid FileId,
    string FileName,
    string FileType,
    long FileSize,
    DateTime CreatedAt
);
