using Backend.Domain.Common;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Notification : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ActionUrl { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        [MaxLength(100)]
        public string? EntityType { get; set; }

        public int? EntityId { get; set; }
    }
}