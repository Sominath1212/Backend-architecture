using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Skills
{
    public class CandidateSkillResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public decimal? YearsOfExperience { get; set; }
        public DateTime? LastUsed { get; set; }
    }
}