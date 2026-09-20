using Backend.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class RecruiterProfile : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        // Navigation property
        public Company Company { get; set; } = null!;

        [MaxLength(150)]
        public string? Designation { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}