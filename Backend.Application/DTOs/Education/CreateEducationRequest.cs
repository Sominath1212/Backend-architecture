using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Education
{
    public class CreateEducationRequest
    {
        [Required]
        [MaxLength(EducationConstants.MaxDegreeLength)]
        public string Degree { get; set; } = string.Empty;

        [Required]
        [MaxLength(EducationConstants.MaxInstitutionLength)]
        public string Institution { get; set; } = string.Empty;

        [MaxLength(EducationConstants.MaxSpecializationLength)]
        public string? Specialization { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int StartYear { get; set; }

        [Range(1900, 2100)]
        public int? EndYear { get; set; }

        public bool IsCurrentlyStudying { get; set; }

        [MaxLength(EducationConstants.MaxGradeLength)]
        public string? Grade { get; set; }
    }
}