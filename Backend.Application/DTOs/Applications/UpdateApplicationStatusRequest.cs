using Backend.Domain.Constants;
using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Applications
{
    public class UpdateApplicationStatusRequest
    {
        [Required]
        public ApplicationStatus Status { get; set; }

        [MaxLength(ApplicationConstants.MaxRecruiterNotesLength)]
        public string? RecruiterNotes { get; set; }

        [MaxLength(500)]
        public string? StatusChangeNotes { get; set; }
    }
}