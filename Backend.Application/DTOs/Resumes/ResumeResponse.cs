namespace Backend.Application.DTOs.Resumes
{
    public class ResumeResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}