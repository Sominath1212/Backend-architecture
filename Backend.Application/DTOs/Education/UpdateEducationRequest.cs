using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Education
{
    public class UpdateEducationRequest
    {
        [MaxLength(EducationConstants.MaxDegreeLength)]
        public string? Degree { get; set; }

        [MaxLength(EducationConstants.MaxInstitutionLength)]
        public string? Institution { get; set; }

        [MaxLength(EducationConstants.MaxSpecializationLength)]
        public string? Specialization { get; set; }

        [Range(1900, 2100)]
        public int? StartYear { get; set; }

        [Range(1900, 2100)]
        public int? EndYear { get; set; }

        public bool? IsCurrentlyStudying { get; set; }

        [MaxLength(EducationConstants.MaxGradeLength)]
        public string? Grade { get; set; }
    }
}