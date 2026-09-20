using Backend.Domain.Enums;

namespace Backend.Application.DTOs.Applications
{
    public class ApplicationStatusHistoryResponse
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }
    }
}