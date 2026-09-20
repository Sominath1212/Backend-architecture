using Backend.Application.DTOs.Skills;
using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Jobs
{
    public class JobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Location { get; set; }
        public WorkMode WorkMode { get; set; }
        public int? NumberOfOpenings { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public JobStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public List<SkillResponse> Skills { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}