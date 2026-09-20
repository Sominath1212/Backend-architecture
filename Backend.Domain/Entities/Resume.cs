using Backend.Domain.Common;
using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class Resume : AuditableEntity
    {
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(ResumeConstants.MaxOriginalFileNameLength)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [Required]
        [MaxLength(ResumeConstants.MaxFileTypeLength)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        [MaxLength(ResumeConstants.MaxStoragePathLength)]
        public string StoragePath { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}