using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Recruiters
{
    public class UpdateRecruiterProfileRequest
    {
        [MaxLength(150)]
        public string? Designation { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}