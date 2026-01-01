namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Result of file deletion operation.
/// </summary>
public record DeleteFileResult(
    bool Success,
    string Message
);
