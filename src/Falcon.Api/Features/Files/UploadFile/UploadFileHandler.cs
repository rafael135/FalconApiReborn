using Falcon.Core.Domain.Files;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;

namespace Falcon.Api.Features.Files.UploadFile;

/// <summary>
/// Handler for uploading files.
/// Validates file extension and size, stores the file using <see cref="IAttachedFileService"/>, and returns metadata about the stored file.
/// </summary>
public class UploadFileHandler : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private readonly IAttachedFileService _attachedFileService;
    private readonly ILogger<UploadFileHandler> _logger;

    private static readonly string[] AllowedExtensions = { ".pdf", ".zip", ".txt", ".md", ".jpg", ".png" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public UploadFileHandler(
        IAttachedFileService attachedFileService,
        ILogger<UploadFileHandler> logger)
    {
        _attachedFileService = attachedFileService;
        _logger = logger;
    }

    /// <summary>
    /// Processes the upload request. Throws <see cref="ArgumentException"/> when the file is missing or invalid.
    /// </summary>
    /// <param name="request">Upload command containing the <see cref="IFormFile"/> to persist.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="UploadFileResult"/> with saved file metadata.</returns>
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

        // Save file using service
        var attachedFile = await _attachedFileService.CreateAttachedFileAsync(
            file.OpenReadStream(),
            file.FileName,
            file.ContentType,
            file.Length,
            cancellationToken
        );

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
