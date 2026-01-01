using Falcon.Core.Domain.Files;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure.Files;

public class AttachedFileService : IAttachedFileService
{
    private readonly FalconDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<AttachedFileService> _logger;

    public AttachedFileService(
        FalconDbContext dbContext,
        IFileStorageService fileStorageService,
        ILogger<AttachedFileService> logger)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<AttachedFile> CreateAttachedFileAsync(Stream fileStream, string fileName, string contentType, long length, CancellationToken cancellationToken = default)
    {
        // 1. Save file to storage
        // We prepend a GUID to the filename to ensure uniqueness, similar to the old service
        // But LocalFileStorageService already does that? Let's check.
        // LocalFileStorageService: var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        // So we just pass the original filename to it?
        // LocalFileStorageService.SaveFileAsync(stream, fileName) uses fileName to get extension.
        
        var relativePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, cancellationToken);

        // 2. Create Entity
        var attachedFile = new AttachedFile(fileName, contentType, length, relativePath);

        // 3. Save to DB
        _dbContext.AttachedFiles.Add(attachedFile);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AttachedFile created: {FileName} (ID: {Id})", fileName, attachedFile.Id);

        return attachedFile;
    }

    public async Task DeleteAttachedFileAsync(AttachedFile file, CancellationToken cancellationToken = default)
    {
        // 1. Delete from storage
        try 
        {
            await _fileStorageService.DeleteFileAsync(file.FilePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from storage: {FilePath}", file.FilePath);
            // We continue to delete from DB even if storage delete fails (or maybe not? Old service threw exception)
            // Old service: throw new ErrorException
            throw; 
        }

        // 2. Delete from DB
        _dbContext.AttachedFiles.Remove(file);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AttachedFile deleted: {FileName} (ID: {Id})", file.Name, file.Id);
    }

    public async Task<AttachedFile> ReplaceAttachedFileAsync(AttachedFile existingFile, Stream newFileStream, string newFileName, string newContentType, long newLength, CancellationToken cancellationToken = default)
    {
        // 1. Delete old
        await DeleteAttachedFileAsync(existingFile, cancellationToken);

        // 2. Create new
        return await CreateAttachedFileAsync(newFileStream, newFileName, newContentType, newLength, cancellationToken);
    }

    public bool IsFileValid(string fileName, long length)
    {
        // Old service: permittedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".pdf";
    }

    public async Task<Stream> GetFileStreamAsync(AttachedFile file, CancellationToken cancellationToken = default)
    {
        return await _fileStorageService.GetFileAsync(file.FilePath, cancellationToken);
    }
}
