using Backend.Application.Common.Models;
using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Jobs
{
    public class JobSearchRequest : PagedRequest
    {
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public WorkMode? WorkMode { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public int? CompanyId { get; set; }
    }
}