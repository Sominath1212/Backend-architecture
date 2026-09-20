using Backend.Domain.Common;
using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class CandidateProfile : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(CandidateProfileConstants.MaxFirstNameLength)]
        public string? FirstName { get; set; }

        [MaxLength(CandidateProfileConstants.MaxLastNameLength)]
        public string? LastName { get; set; }

        [MaxLength(CandidateProfileConstants.MaxEmailLength)]
        public string? Email { get; set; }

        [MaxLength(CandidateProfileConstants.MaxMobileLength)]
        public string? Mobile { get; set; }

        [MaxLength(CandidateProfileConstants.MaxLocationLength)]
        public string? Location { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(CandidateProfileConstants.MaxDesignationLength)]
        public string? CurrentDesignation { get; set; }

        public decimal? TotalExperience { get; set; } // In years

        [MaxLength(CandidateProfileConstants.MaxCompanyLength)]
        public string? CurrentCompany { get; set; }

        public decimal? CurrentSalary { get; set; }

        public decimal? ExpectedSalary { get; set; }

        public int? NoticePeriod { get; set; } // In days

        public EmploymentStatus? EmploymentStatus { get; set; }

        [MaxLength(100)]
        public string? PreferredJobType { get; set; }

        [MaxLength(CandidateProfileConstants.MaxLocationLength)]
        public string? PreferredLocation { get; set; }

        // Navigation property (configured in ApplicationDbContext)
        // public ApplicationUser User { get; set; } = null!;
    }
}