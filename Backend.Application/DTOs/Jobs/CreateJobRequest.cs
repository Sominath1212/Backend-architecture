using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Jobs
{
    public class CreateJobRequest
    {
        [Required]
        [MaxLength(JobConstants.MaxTitleLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(JobConstants.MaxDepartmentLength)]
        public string? Department { get; set; }

        [Required]
        public EmploymentType EmploymentType { get; set; }

        public int? ExperienceMin { get; set; }

        public int? ExperienceMax { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        [MaxLength(JobConstants.MaxLocationLength)]
        public string? Location { get; set; }

        [Required]
        public WorkMode WorkMode { get; set; }

        public int? NumberOfOpenings { get; set; }

        [MaxLength(JobConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        [MaxLength(JobConstants.MaxRequirementsLength)]
        public string? Requirements { get; set; }

        [MaxLength(JobConstants.MaxBenefitsLength)]
        public string? Benefits { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public List<int>? SkillIds { get; set; }
    }
}