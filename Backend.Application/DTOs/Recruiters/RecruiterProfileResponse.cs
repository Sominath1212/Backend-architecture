namespace Backend.Application.DTOs.Recruiters
{
    public class RecruiterProfileResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? Phone { get; set; }
    }
}