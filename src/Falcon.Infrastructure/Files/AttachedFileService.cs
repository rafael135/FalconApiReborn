using Falcon.Core.Domain.Files;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Falcon.Infrastructure.Files;

/// <summary>
/// Provides services for managing <see cref="AttachedFile"/> entities and their underlying file storage.
/// </summary>
public class AttachedFileService : IAttachedFileService
{

    /// <summary>
    /// The database context for accessing attached files.
    /// </summary>
    private readonly FalconDbContext _dbContext;

    /// <summary>
    /// The file storage service for handling physical file operations.
    /// </summary>
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// The logger instance for logging operations.
    /// </summary>
    private readonly ILogger<AttachedFileService> _logger;


    /// <summary>
    /// Initializes a new instance of the <see cref="AttachedFileService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for attached files.</param>
    /// <param name="fileStorageService">The file storage service for file operations.</param>
    /// <param name="logger">The logger instance.</param>
    public AttachedFileService(
        FalconDbContext dbContext,
        IFileStorageService fileStorageService,
        ILogger<AttachedFileService> logger)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Saves a file to storage and creates the corresponding <see cref="AttachedFile"/> entity in the database.
    /// </summary>
    /// <param name="fileStream">The file stream to save.</param>
    /// <param name="fileName">The original file name.</param>
    /// <param name="contentType">The MIME type of the file.</param>
    /// <param name="length">The length of the file in bytes.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>The created <see cref="AttachedFile"/> entity.</returns>
    public async Task<AttachedFile> CreateAttachedFileAsync(Stream fileStream, string fileName, string contentType, long length, CancellationToken cancellationToken = default)
    {
        var relativePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, cancellationToken);
        var attachedFile = new AttachedFile(fileName, contentType, length, relativePath);
        _dbContext.AttachedFiles.Add(attachedFile);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AttachedFile created: {FileName} (ID: {Id})", fileName, attachedFile.Id);
        return attachedFile;
    }

    /// <summary>
    /// Deletes the file from storage and removes the <see cref="AttachedFile"/> entity from the database.
    /// </summary>
    /// <param name="file">The <see cref="AttachedFile"/> entity to delete.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    public async Task DeleteAttachedFileAsync(AttachedFile file, CancellationToken cancellationToken = default)
    {
        try 
        {
            await _fileStorageService.DeleteFileAsync(file.FilePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from storage: {FilePath}", file.FilePath);
            throw; 
        }
        _dbContext.AttachedFiles.Remove(file);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AttachedFile deleted: {FileName} (ID: {Id})", file.Name, file.Id);
    }

    /// <summary>
    /// Replaces an existing file and its entity with a new file and entity.
    /// </summary>
    /// <param name="existingFile">The existing <see cref="AttachedFile"/> entity to replace.</param>
    /// <param name="newFileStream">The stream of the new file.</param>
    /// <param name="newFileName">The name of the new file.</param>
    /// <param name="newContentType">The MIME type of the new file.</param>
    /// <param name="newLength">The length of the new file in bytes.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>The new <see cref="AttachedFile"/> entity.</returns>
    public async Task<AttachedFile> ReplaceAttachedFileAsync(AttachedFile existingFile, Stream newFileStream, string newFileName, string newContentType, long newLength, CancellationToken cancellationToken = default)
    {
        await DeleteAttachedFileAsync(existingFile, cancellationToken);
        return await CreateAttachedFileAsync(newFileStream, newFileName, newContentType, newLength, cancellationToken);
    }

    /// <summary>
    /// Validates if a file is allowed to be uploaded based on its extension and length.
    /// </summary>
    /// <param name="fileName">The name of the file to validate.</param>
    /// <param name="length">The length of the file in bytes.</param>
    /// <returns><c>true</c> if the file is valid; otherwise, <c>false</c>.</returns>
    public bool IsFileValid(string fileName, long length)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".pdf";
    }

    /// <summary>
    /// Retrieves the file stream from storage for the specified <see cref="AttachedFile"/>.
    /// </summary>
    /// <param name="file">The <see cref="AttachedFile"/> entity whose file to retrieve.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A <see cref="Stream"/> containing the file data.</returns>
    public async Task<Stream> GetFileStreamAsync(AttachedFile file, CancellationToken cancellationToken = default)
    {
        return await _fileStorageService.GetFileAsync(file.FilePath, cancellationToken);
    }
}
