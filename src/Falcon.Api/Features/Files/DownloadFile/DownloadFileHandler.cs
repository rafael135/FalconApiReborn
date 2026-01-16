using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Files;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Files.DownloadFile;

/// <summary>
/// Handler for downloading files.
/// </summary>
public class DownloadFileHandler : IRequestHandler<DownloadFileQuery, DownloadFileResult>
{
    private readonly FalconDbContext _context;
    private readonly IAttachedFileService _attachedFileService;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DownloadFileHandler> _logger;

    public DownloadFileHandler(
        FalconDbContext context,
        IAttachedFileService attachedFileService,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DownloadFileHandler> logger
    )
    {
        _context = context;
        _attachedFileService = attachedFileService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the file and returns a <see cref="DownloadFileResult"/>. Throws <see cref="NotFoundException"/> if the file does not exist.
    /// </summary>
    /// <param name="request">The download query carrying the FileId.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream, content type and filename packaged in a <see cref="DownloadFileResult"/>.</returns>
    public async Task<DownloadFileResult> Handle(
        DownloadFileQuery request,
        CancellationToken cancellationToken
    )
    {
        // Get file metadata
        var attachedFile = await _context
            .AttachedFiles.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.FileId, cancellationToken);

        if (attachedFile == null)
        {
            throw new NotFoundException(nameof(AttachedFile), request.FileId);
        }

        // Get file stream using service
        var fileStream = await _attachedFileService.GetFileStreamAsync(
            attachedFile,
            cancellationToken
        );

        // Create log entry
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdClaim = httpContext
            ?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?.Value;

        if (!string.IsNullOrEmpty(userIdClaim))
        {
            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user != null)
            {
                var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                var log = new Log(
                    LogType.DownloadFile,
                    ipAddress,
                    user,
                    user.Group,
                    null // Competition is null for generic file downloads
                );

                _context.Logs.Add(log);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation(
            "File {FileId} ({FileName}) downloaded",
            attachedFile.Id,
            attachedFile.Name
        );

        return new DownloadFileResult(fileStream, attachedFile.Type, attachedFile.Name);
    }
}
