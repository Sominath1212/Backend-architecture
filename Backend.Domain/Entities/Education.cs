using Backend.Domain.Common;
using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Education : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(EducationConstants.MaxDegreeLength)]
        public string Degree { get; set; } = string.Empty;

        [Required]
        [MaxLength(EducationConstants.MaxInstitutionLength)]
        public string Institution { get; set; } = string.Empty;

        [MaxLength(EducationConstants.MaxSpecializationLength)]
        public string? Specialization { get; set; }

        [Required]
        public int StartYear { get; set; }

        public int? EndYear { get; set; }

        public bool IsCurrentlyStudying { get; set; }

        [MaxLength(EducationConstants.MaxGradeLength)]
        public string? Grade { get; set; } // e.g., "8.4 CGPA" or "85%"
    }
}