using Falcon.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure.Storage;

/// <summary>
/// Local file system storage implementation.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseDirectory;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
    {
        _baseDirectory = Path.Combine(environment.WebRootPath, "uploads");
        _logger = logger;

        // Ensure base directory exists
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
            _logger.LogInformation("Created uploads directory at {Path}", _baseDirectory);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        // Generate unique filename
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        // Create year/month directory structure
        var now = DateTime.UtcNow;
        var yearMonth = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"));
        var fullDirectory = Path.Combine(_baseDirectory, yearMonth);

        if (!Directory.Exists(fullDirectory))
        {
            Directory.CreateDirectory(fullDirectory);
        }

        // Full path for saving
        var fullPath = Path.Combine(fullDirectory, uniqueFileName);

        // Save file
        using (var fileStreamOut = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);
        }

        // Return relative path
        var relativePath = Path.Combine(yearMonth, uniqueFileName);
        _logger.LogInformation("Saved file to {Path}", relativePath);

        return relativePath;
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file at {Path}", filePath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }
}
