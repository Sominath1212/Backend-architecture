using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Companies
{
    public class CompanyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public int? CompanySize { get; set; }
        public int? FoundedYear { get; set; }
        public string? Headquarters { get; set; }
        public string? Description { get; set; }
        public string? LogoPath { get; set; }
        public string? CoverImagePath { get; set; }
        public CompanyVerificationStatus VerificationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}