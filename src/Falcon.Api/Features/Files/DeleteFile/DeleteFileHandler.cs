using Falcon.Core.Domain.Files;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Files.DeleteFile;

/// <summary>
/// Handler for deleting files.
/// </summary>
public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, DeleteFileResult>
{
    private readonly FalconDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<DeleteFileHandler> _logger;

    public DeleteFileHandler(
        FalconDbContext context,
        IFileStorageService fileStorage,
        ILogger<DeleteFileHandler> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
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

        // Delete physical file
        await _fileStorage.DeleteFileAsync(attachedFile.FilePath, cancellationToken);

        // Delete database record
        _context.AttachedFiles.Remove(attachedFile);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("File {FileId} ({FileName}) deleted", attachedFile.Id, attachedFile.Name);

        return new DeleteFileResult(true, $"File '{attachedFile.Name}' deleted successfully");
    }
}
