namespace Backend.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<Stream?> DownloadFileAsync(string storagePath, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default);
    }
}