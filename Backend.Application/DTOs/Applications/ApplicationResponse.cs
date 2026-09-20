using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Applications
{
    public class ApplicationResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CandidateUserId { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public int ResumeId { get; set; }
        public string? CoverLetter { get; set; }
        public string? ExpectedSalary { get; set; }
        public string? NoticePeriod { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? RecruiterNotes { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? LastStatusChangedAt { get; set; }
    }
}