using Backend.Domain.Common;
using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class JobAlert : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(JobAlertConstants.MaxKeywordLength)]
        public string? Keyword { get; set; }

        [MaxLength(JobAlertConstants.MaxLocationLength)]
        public string? Location { get; set; }

        public int? ExperienceMin { get; set; }

        public int? ExperienceMax { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public WorkMode? WorkMode { get; set; }

        public EmploymentType? EmploymentType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastCheckedAt { get; set; }
    }
}