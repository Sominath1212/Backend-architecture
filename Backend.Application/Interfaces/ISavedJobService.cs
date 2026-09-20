using Backend.Application.DTOs.SavedJobs;

namespace Backend.Application.Interfaces
{
    public interface ISavedJobService
    {
        Task<IReadOnlyList<SavedJobResponse>> GetSavedJobsAsync(string userId, string traceId);
        Task<SavedJobResponse> SaveJobAsync(string userId, int jobId, string traceId);
        Task UnsaveJobAsync(string userId, int jobId, string traceId);
        Task<bool> IsJobSavedAsync(string userId, int jobId, string traceId);
    }
}