using Backend.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace Backend.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";
            object? errors = null;

            if (exception is Backend.Application.Common.Exceptions.ApplicationException applicationException)
            {
                statusCode = applicationException.StatusCode;
                message = applicationException.Message;
                errors = applicationException.Errors;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.FailureResponse(
                message,
                errors,
                statusCode,
                context.TraceIdentifier);

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}