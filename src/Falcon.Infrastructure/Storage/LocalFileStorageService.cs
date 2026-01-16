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

    /// <summary>
    /// Initializes a new instance of <see cref="LocalFileStorageService"/> using the
    /// provided <see cref="IWebHostEnvironment"/>.
    /// </summary>
    /// <param name="environment">The web host environment containing the web root path.</param>
    /// <param name="logger">The logger instance.</param>
    public LocalFileStorageService(
        IWebHostEnvironment environment,
        ILogger<LocalFileStorageService> logger
    )
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

    /// <summary>
    /// Initializes a new instance of <see cref="LocalFileStorageService"/> using an explicit base directory.
    /// </summary>
    /// <param name="baseDirectory">The base directory where files will be stored.</param>
    /// <param name="logger">The logger instance.</param>
    public LocalFileStorageService(string baseDirectory, ILogger<LocalFileStorageService> logger)
    {
        _baseDirectory = baseDirectory;
        _logger = logger;
        EnsureBaseDirectoryExists();
    }

    /// <summary>
    /// Ensures the base directory exists, creating it if missing.
    /// </summary>
    private void EnsureBaseDirectoryExists()
    {
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
            _logger.LogInformation($"Created uploads directory at {_baseDirectory}");
        }
    }

    /// <summary>
    /// Saves the provided file stream to a unique path under the configured base directory.
    /// The file is stored in a year/month folder and a GUID-prefixed filename is generated.
    /// </summary>
    /// <param name="fileStream">The source stream of the file to save.</param>
    /// <param name="fileName">The original filename (used to infer extension).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The relative path where the file was saved.</returns>
    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    )
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

    /// <summary>
    /// Retrieves a file from storage as a <see cref="Stream"/>.
    /// </summary>
    /// <param name="filePath">The relative path of the file to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the file contents.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist at the expected path.</exception>
    public async Task<Stream> GetFileAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
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

    /// <summary>
    /// Deletes a file from local storage if it exists.
    /// </summary>
    /// <param name="filePath">The relative path of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token (not used but provided for interface completeness).</param>
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

    /// <summary>
    /// Checks whether a file exists in storage.
    /// </summary>
    /// <param name="filePath">The relative path of the file.</param>
    /// <param name="cancellationToken">Cancellation token (not used but provided for interface completeness).</param>
    /// <returns>True if the file exists; otherwise false.</returns>
    public Task<bool> FileExistsAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = Path.Combine(_baseDirectory, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }
}
