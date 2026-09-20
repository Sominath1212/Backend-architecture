using Backend.Application.DTOs.JobAlerts;

namespace Backend.Application.Interfaces
{
    public interface IJobAlertService
    {
        Task<IReadOnlyList<JobAlertResponse>> GetMyAlertsAsync(string userId, string traceId);
        Task<JobAlertResponse> GetByIdAsync(string userId, int alertId, string traceId);
        Task<JobAlertResponse> CreateAsync(string userId, CreateJobAlertRequest request, string traceId);
        Task<JobAlertResponse> UpdateAsync(string userId, int alertId, UpdateJobAlertRequest request, string traceId);
        Task DeleteAsync(string userId, int alertId, string traceId);
        Task EnableAsync(string userId, int alertId, string traceId);
        Task DisableAsync(string userId, int alertId, string traceId);
    }
}