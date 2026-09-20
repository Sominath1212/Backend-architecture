using Backend.Application.DTOs.Jobs;

namespace Backend.Application.DTOs.SavedJobs
{
    public class SavedJobResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public JobResponse Job { get; set; } = null!;
        public DateTime SavedAt { get; set; }
    }
}