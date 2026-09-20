using Backend.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class JobSkill : AuditableEntity
    {
        [Required]
        public int JobId { get; set; }

        // Navigation property
        public Job Job { get; set; } = null!;

        [Required]
        public int SkillId { get; set; }

        // Navigation property
        public Skill Skill { get; set; } = null!;
    }
}