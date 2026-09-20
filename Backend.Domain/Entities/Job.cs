using Backend.Domain.Common;
using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Job : AuditableEntity
    {
        [Required]
        [MaxLength(JobConstants.MaxTitleLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        // Navigation property
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string PostedByUserId { get; set; } = string.Empty;

        [MaxLength(JobConstants.MaxDepartmentLength)]
        public string? Department { get; set; }

        public EmploymentType EmploymentType { get; set; }

        public int? ExperienceMin { get; set; }

        public int? ExperienceMax { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        [MaxLength(JobConstants.MaxLocationLength)]
        public string? Location { get; set; }

        public WorkMode WorkMode { get; set; }

        public int? NumberOfOpenings { get; set; }

        [MaxLength(JobConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        [MaxLength(JobConstants.MaxRequirementsLength)]
        public string? Requirements { get; set; }

        [MaxLength(JobConstants.MaxBenefitsLength)]
        public string? Benefits { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Draft;

        public bool IsFeatured { get; set; }

        // Navigation property
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}