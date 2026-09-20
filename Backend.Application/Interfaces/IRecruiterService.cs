using Backend.Application.DTOs.Recruiters;

namespace Backend.Application.Interfaces
{
    public interface IRecruiterService
    {
        Task<IReadOnlyList<RecruiterProfileResponse>> GetByUserIdAsync(string userId, string traceId);
        Task<IReadOnlyList<RecruiterProfileResponse>> GetByCompanyIdAsync(int companyId, string traceId);
        Task<RecruiterProfileResponse> CreateAsync(string userId, CreateRecruiterProfileRequest request, string traceId);
        Task<RecruiterProfileResponse> UpdateAsync(string userId, int recruiterId, UpdateRecruiterProfileRequest request, string traceId);
        Task DeleteAsync(string userId, int recruiterId, string traceId);
    }
}