using Backend.Domain.Common;
using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Experience : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(ExperienceConstants.MaxCompanyLength)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(ExperienceConstants.MaxDesignationLength)]
        public string Designation { get; set; } = string.Empty;

        [Required]
        public EmploymentType EmploymentType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsCurrentlyWorking { get; set; }

        [MaxLength(ExperienceConstants.MaxLocationLength)]
        public string? Location { get; set; }

        [MaxLength(ExperienceConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        [MaxLength(ExperienceConstants.MaxSkillsUsedLength)]
        public string? SkillsUsed { get; set; } // Comma-separated or JSON, depending on frontend handling
    }
}