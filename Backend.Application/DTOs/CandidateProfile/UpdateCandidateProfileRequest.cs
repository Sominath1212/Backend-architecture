using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.CandidateProfile
{
    public class UpdateCandidateProfileRequest
    {
        [MaxLength(CandidateProfileConstants.MaxFirstNameLength)]
        public string? FirstName { get; set; }

        [MaxLength(CandidateProfileConstants.MaxLastNameLength)]
        public string? LastName { get; set; }

        [MaxLength(CandidateProfileConstants.MaxMobileLength)]
        public string? Mobile { get; set; }

        [MaxLength(CandidateProfileConstants.MaxLocationLength)]
        public string? Location { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(CandidateProfileConstants.MaxDesignationLength)]
        public string? CurrentDesignation { get; set; }

        public decimal? TotalExperience { get; set; }

        [MaxLength(CandidateProfileConstants.MaxCompanyLength)]
        public string? CurrentCompany { get; set; }

        public decimal? CurrentSalary { get; set; }

        public decimal? ExpectedSalary { get; set; }

        public int? NoticePeriod { get; set; }

        public EmploymentStatus? EmploymentStatus { get; set; }

        [MaxLength(100)]
        public string? PreferredJobType { get; set; }

        [MaxLength(CandidateProfileConstants.MaxLocationLength)]
        public string? PreferredLocation { get; set; }
    }
}