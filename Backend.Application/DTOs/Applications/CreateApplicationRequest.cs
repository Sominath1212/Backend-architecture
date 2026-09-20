using Backend.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Applications
{
    public class CreateApplicationRequest
    {
        [Required]
        public int JobId { get; set; }

        [Required]
        public int ResumeId { get; set; }

        [MaxLength(ApplicationConstants.MaxCoverLetterLength)]
        public string? CoverLetter { get; set; }

        [MaxLength(ApplicationConstants.MaxExpectedSalaryLength)]
        public string? ExpectedSalary { get; set; }

        [MaxLength(ApplicationConstants.MaxNoticePeriodLength)]
        public string? NoticePeriod { get; set; }
    }
}