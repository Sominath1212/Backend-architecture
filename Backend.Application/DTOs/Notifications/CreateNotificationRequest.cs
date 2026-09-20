using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Notifications
{
    public class CreateNotificationRequest
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

        [MaxLength(100)]
        public string? EntityType { get; set; }

        public int? EntityId { get; set; }
    }
}