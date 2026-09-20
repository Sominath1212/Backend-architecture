using Backend.Domain.Enums;

namespace Backend.Application.DTOs.CandidateProfile
{
    public class CandidateProfileResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Location { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? CurrentDesignation { get; set; }
        public decimal? TotalExperience { get; set; }
        public string? CurrentCompany { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public int? NoticePeriod { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }
        public string? PreferredJobType { get; set; }
        public string? PreferredLocation { get; set; }
    }
}