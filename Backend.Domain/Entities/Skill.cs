using Backend.Domain.Common;
using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Skill : AuditableEntity
    {
        [Required]
        [MaxLength(SkillConstants.MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}