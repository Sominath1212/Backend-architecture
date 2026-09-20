using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Resumes;
using Backend.Application.Interfaces;
using Backend.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/candidates/me/[controller]")]
    public class ResumesController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly ILogger<ResumesController> _logger;

        public ResumesController(
            IResumeService resumeService,
            ILogger<ResumesController> logger)
        {
            _resumeService = resumeService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub")
                   ?? throw new UnauthorizedAccessException("User ID not found in token.");
        }

        private string GetTraceId()
        {
            return Activity.Current?.TraceId.ToString() ?? HttpContext.TraceIdentifier;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetAll Resumes. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _resumeService.GetAllByUserIdAsync(userId, traceId);

            _logger.LogInformation("Completed GetAll Resumes successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<ResumeResponse>>.SuccessResponse(result, "Resumes retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            if (file is null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded. TraceId: {TraceId}", traceId);
                return BadRequest(ApiResponse<string>.FailureResponse("No file uploaded.", StatusCodes.Status400BadRequest));
            }

            if (file.Length > ResumeConstants.MaxFileSizeBytes)
            {
                _logger.LogWarning("File size exceeds limit. FileSize: {FileSize}, TraceId: {TraceId}", file.Length, traceId);
                return BadRequest(ApiResponse<string>.FailureResponse($"File size exceeds limit of {ResumeConstants.MaxFileSizeBytes / (1024 * 1024)} MB.", StatusCodes.Status400BadRequest));
            }

            _logger.LogInformation("Starting Upload Resume. UserId: {UserId}, FileName: {FileName}, TraceId: {TraceId}", userId, file.FileName, traceId);

            using var stream = file.OpenReadStream();
            var result = await _resumeService.UploadResumeAsync(userId, stream, file.FileName, file.ContentType, traceId);

            _logger.LogInformation("Completed Upload Resume successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ResumeResponse>.SuccessResponse(result, "Resume uploaded successfully.", StatusCodes.Status201Created));
        }

        [HttpPost("{resumeId:int}/set-primary")]
        public async Task<IActionResult> SetPrimary(int resumeId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting SetPrimary Resume. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);

            var result = await _resumeService.SetPrimaryAsync(userId, resumeId, traceId);

            _logger.LogInformation("Completed SetPrimary Resume successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ResumeResponse>.SuccessResponse(result, "Resume set as primary successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("{resumeId:int}/download")]
        public async Task<IActionResult> Download(int resumeId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Download Resume. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);

            var (fileStream, fileName, contentType) = await _resumeService.DownloadResumeAsync(userId, resumeId, traceId);

            _logger.LogInformation("Completed Download Resume successfully. TraceId: {TraceId}", traceId);
            return File(fileStream, contentType, fileName);
        }

        [HttpDelete("{resumeId:int}")]
        public async Task<IActionResult> Delete(int resumeId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Resume. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);

            await _resumeService.DeleteResumeAsync(userId, resumeId, traceId);

            _logger.LogInformation("Completed Delete Resume successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Resume deleted successfully.", StatusCodes.Status200OK));
        }
    }
}