using Backend.Application.Common.Models;
using Backend.Application.DTOs.Applications;

namespace Backend.Application.Interfaces
{
    public interface IApplicationService
    {
        // Candidate operations
        Task<ApplicationResponse> ApplyAsync(string candidateUserId, CreateApplicationRequest request, string traceId);
        Task<IReadOnlyList<ApplicationResponse>> GetMyApplicationsAsync(string candidateUserId, string traceId);
        Task<ApplicationResponse> GetMyApplicationByIdAsync(string candidateUserId, int applicationId, string traceId);
        Task WithdrawAsync(string candidateUserId, int applicationId, string traceId);

        // Recruiter operations
        Task<PagedResult<ApplicationResponse>> GetApplicationsByJobIdAsync(string recruiterUserId, int jobId, string traceId);
        Task<ApplicationResponse> GetApplicationByIdAsync(string recruiterUserId, int applicationId, string traceId);
        Task<ApplicationResponse> UpdateStatusAsync(string recruiterUserId, int applicationId, UpdateApplicationStatusRequest request, string traceId);
        Task<IReadOnlyList<ApplicationStatusHistoryResponse>> GetStatusHistoryAsync(string recruiterUserId, int applicationId, string traceId);
    }
}