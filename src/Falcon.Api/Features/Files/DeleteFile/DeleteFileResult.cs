namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Result of file deletion operation.
/// </summary>
/// <param name="Success">Whether the deletion succeeded.</param>
/// <param name="Message">Human-readable message about the operation.</param>
public record DeleteFileResult(bool Success, string Message);
