using Backend.Domain.Common;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class CandidateSkill : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int SkillId { get; set; }

        // Navigation property
        public Skill Skill { get; set; } = null!;

        public ProficiencyLevel ProficiencyLevel { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public DateTime? LastUsed { get; set; }
    }
}