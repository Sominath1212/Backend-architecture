using Backend.Domain.Common;
using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Company : AuditableEntity
    {
        [Required]
        [MaxLength(CompanyConstants.MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(CompanyConstants.MaxWebsiteLength)]
        public string? Website { get; set; }

        [MaxLength(CompanyConstants.MaxIndustryLength)]
        public string? Industry { get; set; }

        public int? CompanySize { get; set; } // Number of employees

        public int? FoundedYear { get; set; }

        [MaxLength(CompanyConstants.MaxHeadquartersLength)]
        public string? Headquarters { get; set; }

        [MaxLength(CompanyConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        [MaxLength(CompanyConstants.MaxLogoPathLength)]
        public string? LogoPath { get; set; }

        [MaxLength(CompanyConstants.MaxCoverImagePathLength)]
        public string? CoverImagePath { get; set; }

        public CompanyVerificationStatus VerificationStatus { get; set; } = CompanyVerificationStatus.Pending;

        public string? VerificationNotes { get; set; }

        // Navigation property
        public ICollection<RecruiterProfile> Recruiters { get; set; } = new List<RecruiterProfile>();
    }
}