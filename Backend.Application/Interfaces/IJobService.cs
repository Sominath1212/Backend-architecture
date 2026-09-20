using Backend.Application.Common.Models;
using Backend.Application.DTOs.Jobs;

namespace Backend.Application.Interfaces
{
    public interface IJobService
    {
        Task<PagedResult<JobResponse>> SearchJobsAsync(JobSearchRequest request, string traceId);
        Task<JobResponse> GetByIdAsync(int jobId, string traceId);
        Task<JobResponse> CreateAsync(string userId, CreateJobRequest request, string traceId);
        Task<JobResponse> UpdateAsync(string userId, int jobId, UpdateJobRequest request, string traceId);
        Task ChangeStatusAsync(string userId, int jobId, string newStatus, string traceId);
        Task DeleteAsync(string userId, int jobId, string traceId);
    }
}