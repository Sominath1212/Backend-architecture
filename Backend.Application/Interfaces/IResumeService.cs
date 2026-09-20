using Backend.Application.DTOs.Resumes;

namespace Backend.Application.Interfaces
{
    public interface IResumeService
    {
        Task<IReadOnlyList<ResumeResponse>> GetAllByUserIdAsync(string userId, string traceId);
        Task<ResumeResponse> UploadResumeAsync(string userId, Stream fileStream, string fileName, string contentType, string traceId);
        Task<ResumeResponse> SetPrimaryAsync(string userId, int resumeId, string traceId);
        Task<(Stream FileStream, string FileName, string ContentType)> DownloadResumeAsync(string userId, int resumeId, string traceId);
        Task DeleteResumeAsync(string userId, int resumeId, string traceId);
    }
}