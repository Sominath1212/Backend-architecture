using Backend.Application.DTOs.Education;

namespace Backend.Application.Interfaces
{
    public interface IEducationService
    {
        Task<IReadOnlyList<EducationResponse>> GetAllByUserIdAsync(string userId, string traceId);
        Task<EducationResponse> GetByIdAsync(string userId, int educationId, string traceId);
        Task<EducationResponse> CreateAsync(string userId, CreateEducationRequest request, string traceId);
        Task<EducationResponse> UpdateAsync(string userId, int educationId, UpdateEducationRequest request, string traceId);
        Task DeleteAsync(string userId, int educationId, string traceId);
    }
}