using Backend.Domain.Enums;

namespace Backend.Application.DTOs.JobAlerts
{
    public class JobAlertResponse
    {
        public int Id { get; set; }
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public WorkMode? WorkMode { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastCheckedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}