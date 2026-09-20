using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.JobAlerts
{
    public class CreateJobAlertRequest
    {
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
    }
}