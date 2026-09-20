namespace Backend.Application.Common.Models
{


        public class ApiResponse<T>
        {
            public bool Success { get; set; }

            public int StatusCode { get; set; }

            public string Message { get; set; } = string.Empty;

            public T? Data { get; set; }

            public object? Errors { get; set; }

            public string? TraceId { get; set; }

            public static ApiResponse<T> SuccessResponse(
                T data,
                string message = "Request successful",
                int statusCode = 200)
            {
                return new ApiResponse<T>
                {
                    Success = true,
                    StatusCode = statusCode,
                    Message = message,
                    Data = data
                };
            }

            public static ApiResponse<T> FailureResponse(
                string message,
                object? errors = null,
                int statusCode = 400,
                string? traceId = null)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors,
                    TraceId = traceId
                };
            }
        }
}