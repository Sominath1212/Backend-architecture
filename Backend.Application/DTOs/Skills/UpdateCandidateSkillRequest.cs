using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Skills
{
    public class UpdateCandidateSkillRequest
    {
        public ProficiencyLevel? ProficiencyLevel { get; set; }

        [Range(0, 50)]
        public decimal? YearsOfExperience { get; set; }

        public DateTime? LastUsed { get; set; }
    }
}