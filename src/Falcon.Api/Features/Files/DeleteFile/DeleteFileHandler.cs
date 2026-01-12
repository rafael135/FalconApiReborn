using Falcon.Core.Domain.Files;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Handler for deleting files.
/// Ensures the file exists and is not attached to any exercises before deleting the physical file and database record.
/// </summary>
public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, DeleteFileResult>
{
    private readonly FalconDbContext _context;
    private readonly IAttachedFileService _attachedFileService;
    private readonly ILogger<DeleteFileHandler> _logger;

    /// <summary>
    /// Deletes the file record and physical storage. Throws <see cref="NotFoundException"/> if file is not found,
    /// or <see cref="InvalidOperationException"/> if the file is still attached to exercises.
    /// </summary>
    public DeleteFileHandler(
        FalconDbContext context,
        IAttachedFileService attachedFileService,
        ILogger<DeleteFileHandler> logger)
    {
        _context = context;
        _attachedFileService = attachedFileService;
        _logger = logger;
    }

    public async Task<DeleteFileResult> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var attachedFile = await _context.AttachedFiles
            .Include(f => f.Exercises)
            .FirstOrDefaultAsync(f => f.Id == request.FileId, cancellationToken);

        if (attachedFile == null)
        {
            throw new NotFoundException(nameof(AttachedFile), request.FileId);
        }

        // Check if file is attached to any exercises
        if (attachedFile.Exercises.Any())
        {
            throw new InvalidOperationException(
                $"Cannot delete file '{attachedFile.Name}' because it is attached to {attachedFile.Exercises.Count} exercise(s)");
        }

        // Delete using service
        await _attachedFileService.DeleteAttachedFileAsync(attachedFile, cancellationToken);

        _logger.LogInformation("File {FileId} ({FileName}) deleted", attachedFile.Id, attachedFile.Name);

        return new DeleteFileResult(true, $"File '{attachedFile.Name}' deleted successfully");
    }
}
