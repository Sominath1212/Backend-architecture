using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Experience
{
    public class ExperienceResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public EmploymentType EmploymentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentlyWorking { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? SkillsUsed { get; set; }
    }
}