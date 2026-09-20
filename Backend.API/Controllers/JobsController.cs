using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Jobs;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
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

        // Public: Search & list published jobs
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] JobSearchRequest request)
        {
            var traceId = GetTraceId();
            _logger.LogInformation("Starting Search Jobs. TraceId: {TraceId}", traceId);

            var result = await _jobService.SearchJobsAsync(request, traceId);

            _logger.LogInformation("Completed Search Jobs successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<PagedResult<JobResponse>>.SuccessResponse(result, "Jobs retrieved successfully.", StatusCodes.Status200OK));
        }

        // Public: Get single job details
        [AllowAnonymous]
        [HttpGet("{jobId:int}")]
        public async Task<IActionResult> GetById(int jobId)
        {
            var traceId = GetTraceId();
            _logger.LogInformation("Starting GetById Job. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);

            var result = await _jobService.GetByIdAsync(jobId, traceId);

            _logger.LogInformation("Completed GetById Job successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<JobResponse>.SuccessResponse(result, "Job retrieved successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Create job
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Create Job. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _jobService.CreateAsync(userId, request, traceId);

            _logger.LogInformation("Completed Create Job successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<JobResponse>.SuccessResponse(result, "Job created successfully.", StatusCodes.Status201Created));
        }

        // Recruiter: Update job
        [Authorize]
        [HttpPut("{jobId:int}")]
        public async Task<IActionResult> Update(int jobId, [FromBody] UpdateJobRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Update Job. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

            var result = await _jobService.UpdateAsync(userId, jobId, request, traceId);

            _logger.LogInformation("Completed Update Job successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<JobResponse>.SuccessResponse(result, "Job updated successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Change job status (publish, pause, close, etc.)
        [Authorize]
        [HttpPost("{jobId:int}/status")]
        public async Task<IActionResult> ChangeStatus(int jobId, [FromQuery] string status)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting ChangeStatus Job. UserId: {UserId}, JobId: {JobId}, Status: {Status}, TraceId: {TraceId}", userId, jobId, status, traceId);

            await _jobService.ChangeStatusAsync(userId, jobId, status, traceId);

            _logger.LogInformation("Completed ChangeStatus Job successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, $"Job status changed to {status} successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Delete job
        [Authorize]
        [HttpDelete("{jobId:int}")]
        public async Task<IActionResult> Delete(int jobId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Job. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

            await _jobService.DeleteAsync(userId, jobId, traceId);

            _logger.LogInformation("Completed Delete Job successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Job deleted successfully.", StatusCodes.Status200OK));
        }
    }
}