using Backend.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastucture.Services.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadDirectory;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "resumes");

        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

        _logger.LogInformation("Uploading file to local storage. FilePath: {FilePath}", filePath);

        using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);
        }

        return filePath;
    }

    public async Task<Stream?> DownloadFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath))
        {
            _logger.LogWarning("File not found at path: {StoragePath}", storagePath);
            return null;
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(storagePath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(storagePath))
        {
            _logger.LogInformation("Deleting file from local storage. FilePath: {StoragePath}", storagePath);
            File.Delete(storagePath);
        }

        return Task.CompletedTask;
    }
}