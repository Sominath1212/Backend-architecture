using Backend.Application.DTOs.Experience;

namespace Backend.Application.Interfaces
{
    public interface IExperienceService
    {
        Task<IReadOnlyList<ExperienceResponse>> GetAllByUserIdAsync(string userId, string traceId);
        Task<ExperienceResponse> GetByIdAsync(string userId, int experienceId, string traceId);
        Task<ExperienceResponse> CreateAsync(string userId, CreateExperienceRequest request, string traceId);
        Task<ExperienceResponse> UpdateAsync(string userId, int experienceId, UpdateExperienceRequest request, string traceId);
        Task DeleteAsync(string userId, int experienceId, string traceId);
    }
}