using Falcon.Core.Domain.Files;

namespace Falcon.Core.Interfaces;

/// <summary>
/// Service for managing AttachedFile entities and their underlying storage.
/// </summary>
public interface IAttachedFileService
{
    /// <summary>
    /// Saves a file to storage and creates the corresponding database entity.
    /// </summary>
    Task<AttachedFile> CreateAttachedFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes the file from storage and removes the entity from the database.
    /// </summary>
    Task DeleteAttachedFileAsync(AttachedFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces an existing file with a new one, updating the entity and storage.
    /// </summary>
    Task<AttachedFile> ReplaceAttachedFileAsync(
        AttachedFile existingFile,
        Stream newFileStream,
        string newFileName,
        string newContentType,
        long newLength,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates if a file is allowed to be uploaded.
    /// </summary>
    bool IsFileValid(string fileName, long length);

    /// <summary>
    /// Retrieves the file stream from storage.
    /// </summary>
    Task<Stream> GetFileStreamAsync(
        AttachedFile file,
        CancellationToken cancellationToken = default
    );
}
