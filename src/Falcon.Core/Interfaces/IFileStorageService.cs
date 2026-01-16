namespace Falcon.Core.Interfaces;

/// <summary>
/// Service for handling file storage operations.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to storage and returns the relative path.
    /// </summary>
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Reads a file from storage.
    /// </summary>
    Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in storage.
    /// </summary>
    Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
}
