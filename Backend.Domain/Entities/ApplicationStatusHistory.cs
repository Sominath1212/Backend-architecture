using Backend.Domain.Common;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class ApplicationStatusHistory : AuditableEntity
    {
        [Required]
        public int ApplicationId { get; set; }

        // Navigation property
        public JobApplication Application { get; set; } = null!;

        [Required]
        public ApplicationStatus Status { get; set; }

        [MaxLength(256)]
        public string? ChangedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}