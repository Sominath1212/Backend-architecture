namespace Backend.Application.Common.Exceptions
{
    public class ApplicationException : Exception
    {
        public int StatusCode { get; }

        public object? Errors { get; }

        public ApplicationException(
            string message,
            int statusCode = 400,
            object? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
