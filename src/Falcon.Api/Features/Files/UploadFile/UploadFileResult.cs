namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Result of file upload operation.
/// </summary>
public record UploadFileResult(
    Guid FileId,
    string FileName,
    string FileType,
    long FileSize,
    DateTime CreatedAt
);
