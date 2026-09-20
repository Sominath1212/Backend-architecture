namespace Backend.Domain.Constants
{
    public static class ResumeConstants
    {
        public const int MaxOriginalFileNameLength = 255;
        public const int MaxStoragePathLength = 500;
        public const int MaxFileTypeLength = 50;
        public const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public static readonly string[] AllowedFileTypes = { ".pdf", ".doc", ".docx" };
    }
}