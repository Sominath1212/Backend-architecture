using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.SavedJobs;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/candidates/me/saved-jobs")]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobService _savedJobService;
        private readonly ILogger<SavedJobsController> _logger;

        public SavedJobsController(ISavedJobService savedJobService, ILogger<SavedJobsController> logger)
        {
            _savedJobService = savedJobService;
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
        public async Task<IActionResult> GetSavedJobs()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetSavedJobs. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _savedJobService.GetSavedJobsAsync(userId, traceId);

            _logger.LogInformation("Completed GetSavedJobs successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<SavedJobResponse>>.SuccessResponse(result, "Saved jobs retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("{jobId:int}")]
        public async Task<IActionResult> SaveJob(int jobId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting SaveJob. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

            var result = await _savedJobService.SaveJobAsync(userId, jobId, traceId);

            _logger.LogInformation("Completed SaveJob successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<SavedJobResponse>.SuccessResponse(result, "Job saved successfully.", StatusCodes.Status201Created));
        }

        [HttpDelete("{jobId:int}")]
        public async Task<IActionResult> UnsaveJob(int jobId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting UnsaveJob. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

            await _savedJobService.UnsaveJobAsync(userId, jobId, traceId);

            _logger.LogInformation("Completed UnsaveJob successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Job unsaved successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("{jobId:int}/check")]
        public async Task<IActionResult> IsJobSaved(int jobId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting IsJobSaved. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

            var isSaved = await _savedJobService.IsJobSavedAsync(userId, jobId, traceId);

            _logger.LogInformation("Completed IsJobSaved successfully. Result: {Result}, TraceId: {TraceId}", isSaved, traceId);
            return Ok(ApiResponse<bool>.SuccessResponse(isSaved, isSaved ? "Job is saved." : "Job is not saved.", StatusCodes.Status200OK));
        }
    }
}