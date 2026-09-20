using Backend.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class SavedJob : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int JobId { get; set; }

        // Navigation property
        public Job Job { get; set; } = null!;
    }
}