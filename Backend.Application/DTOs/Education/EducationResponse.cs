namespace Backend.Application.DTOs.Education
{
    public class EducationResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public bool IsCurrentlyStudying { get; set; }
        public string? Grade { get; set; }
    }
}