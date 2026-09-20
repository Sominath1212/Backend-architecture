using Backend.Domain.Common;
using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class JobApplication : AuditableEntity
    {
        [Required]
        public int JobId { get; set; }

        // Navigation property
        public Job Job { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string CandidateUserId { get; set; } = string.Empty;

        [Required]
        public int? ResumeId { get; set; }

        // Navigation property
        public Resume? Resume { get; set; }

        [MaxLength(ApplicationConstants.MaxCoverLetterLength)]
        public string? CoverLetter { get; set; }

        [MaxLength(ApplicationConstants.MaxExpectedSalaryLength)]
        public string? ExpectedSalary { get; set; }

        [MaxLength(ApplicationConstants.MaxNoticePeriodLength)]
        public string? NoticePeriod { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

        [MaxLength(ApplicationConstants.MaxRecruiterNotesLength)]
        public string? RecruiterNotes { get; set; }

        // Navigation property
        public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
    }
}