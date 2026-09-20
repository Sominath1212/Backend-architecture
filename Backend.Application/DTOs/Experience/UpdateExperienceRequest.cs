using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Experience
{
    public class UpdateExperienceRequest
    {
        [MaxLength(ExperienceConstants.MaxCompanyLength)]
        public string? Company { get; set; }

        [MaxLength(ExperienceConstants.MaxDesignationLength)]
        public string? Designation { get; set; }

        public EmploymentType? EmploymentType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsCurrentlyWorking { get; set; }

        [MaxLength(ExperienceConstants.MaxLocationLength)]
        public string? Location { get; set; }

        [MaxLength(ExperienceConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        [MaxLength(ExperienceConstants.MaxSkillsUsedLength)]
        public string? SkillsUsed { get; set; }
    }
}