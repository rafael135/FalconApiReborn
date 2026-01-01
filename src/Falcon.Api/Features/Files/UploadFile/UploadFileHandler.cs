using Falcon.Core.Domain.Files;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;

namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Handler for uploading files.
/// </summary>
public class UploadFileHandler : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private readonly FalconDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<UploadFileHandler> _logger;

    private static readonly string[] AllowedExtensions = { ".pdf", ".zip", ".txt", ".md", ".jpg", ".png" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public UploadFileHandler(
        FalconDbContext context,
        IFileStorageService fileStorage,
        ILogger<UploadFileHandler> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<UploadFileResult> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;

        // Validation
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required and cannot be empty");
        }

        // Validate extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");
        }

        // Validate size
        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)} MB");
        }

        // Save file to storage
        string filePath;
        using (var stream = file.OpenReadStream())
        {
            filePath = await _fileStorage.SaveFileAsync(stream, file.FileName, cancellationToken);
        }

        // Create AttachedFile entity
        var attachedFile = new AttachedFile(
            file.FileName,
            file.ContentType,
            file.Length,
            filePath
        );

        _context.AttachedFiles.Add(attachedFile);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("File {FileName} uploaded with ID {FileId}", file.FileName, attachedFile.Id);

        return new UploadFileResult(
            attachedFile.Id,
            attachedFile.Name,
            attachedFile.Type,
            attachedFile.Size,
            attachedFile.CreatedAt
        );
    }
}
