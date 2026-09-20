using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Companies
{
    public class CreateCompanyRequest
    {
        [Required]
        [MaxLength(CompanyConstants.MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(CompanyConstants.MaxWebsiteLength)]
        public string? Website { get; set; }

        [MaxLength(CompanyConstants.MaxIndustryLength)]
        public string? Industry { get; set; }

        public int? CompanySize { get; set; }

        public int? FoundedYear { get; set; }

        [MaxLength(CompanyConstants.MaxHeadquartersLength)]
        public string? Headquarters { get; set; }

        [MaxLength(CompanyConstants.MaxDescriptionLength)]
        public string? Description { get; set; }
    }
}