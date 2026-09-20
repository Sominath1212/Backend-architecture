using Backend.Application.DTOs.Skills;

namespace Backend.Application.Interfaces
{
    public interface ISkillService
    {
        // Candidate Skills
        Task<IReadOnlyList<CandidateSkillResponse>> GetCandidateSkillsAsync(string userId, string traceId);
        Task<CandidateSkillResponse> CreateCandidateSkillAsync(string userId, CreateCandidateSkillRequest request, string traceId);
        Task<CandidateSkillResponse> UpdateCandidateSkillAsync(string userId, int candidateSkillId, UpdateCandidateSkillRequest request, string traceId);
        Task DeleteCandidateSkillAsync(string userId, int candidateSkillId, string traceId);

        // Master Skills (for dropdowns)
        Task<IReadOnlyList<SkillResponse>> GetAllMasterSkillsAsync(string traceId);
    }
}