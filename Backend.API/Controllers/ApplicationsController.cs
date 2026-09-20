using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Applications;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(IApplicationService applicationService, ILogger<ApplicationsController> logger)
        {
            _applicationService = applicationService;
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

        // Candidate: Apply to a job
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] CreateApplicationRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Apply. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _applicationService.ApplyAsync(userId, request, traceId);

            _logger.LogInformation("Completed Apply successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ApplicationResponse>.SuccessResponse(result, "Application submitted successfully.", StatusCodes.Status201Created));
        }

        // Candidate: Get my applications
        [HttpGet("me")]
        public async Task<IActionResult> GetMyApplications()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMyApplications. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _applicationService.GetMyApplicationsAsync(userId, traceId);

            _logger.LogInformation("Completed GetMyApplications successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<ApplicationResponse>>.SuccessResponse(result, "Applications retrieved successfully.", StatusCodes.Status200OK));
        }

        // Candidate: Get my application by ID
        [HttpGet("me/{applicationId:int}")]
        public async Task<IActionResult> GetMyApplicationById(int applicationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMyApplicationById. UserId: {UserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", userId, applicationId, traceId);

            var result = await _applicationService.GetMyApplicationByIdAsync(userId, applicationId, traceId);

            _logger.LogInformation("Completed GetMyApplicationById successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ApplicationResponse>.SuccessResponse(result, "Application retrieved successfully.", StatusCodes.Status200OK));
        }

        // Candidate: Withdraw application
        [HttpPost("{applicationId:int}/withdraw")]
        public async Task<IActionResult> Withdraw(int applicationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Withdraw. UserId: {UserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", userId, applicationId, traceId);

            await _applicationService.WithdrawAsync(userId, applicationId, traceId);

            _logger.LogInformation("Completed Withdraw successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Application withdrawn successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Get applications for a job
        [HttpGet("job/{jobId:int}")]
        public async Task<IActionResult> GetApplicationsByJobId(int jobId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetApplicationsByJobId. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

            var result = await _applicationService.GetApplicationsByJobIdAsync(userId, jobId, traceId);

            _logger.LogInformation("Completed GetApplicationsByJobId successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<PagedResult<ApplicationResponse>>.SuccessResponse(result, "Applications retrieved successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Get application by ID
        [HttpGet("{applicationId:int}")]
        public async Task<IActionResult> GetApplicationById(int applicationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetApplicationById. UserId: {UserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", userId, applicationId, traceId);

            var result = await _applicationService.GetApplicationByIdAsync(userId, applicationId, traceId);

            _logger.LogInformation("Completed GetApplicationById successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ApplicationResponse>.SuccessResponse(result, "Application retrieved successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Update application status
        [HttpPut("{applicationId:int}/status")]
        public async Task<IActionResult> UpdateStatus(int applicationId, [FromBody] UpdateApplicationStatusRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting UpdateStatus. UserId: {UserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", userId, applicationId, traceId);

            var result = await _applicationService.UpdateStatusAsync(userId, applicationId, request, traceId);

            _logger.LogInformation("Completed UpdateStatus successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ApplicationResponse>.SuccessResponse(result, "Application status updated successfully.", StatusCodes.Status200OK));
        }

        // Recruiter: Get application status history
        [HttpGet("{applicationId:int}/history")]
        public async Task<IActionResult> GetStatusHistory(int applicationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetStatusHistory. UserId: {UserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", userId, applicationId, traceId);

            var result = await _applicationService.GetStatusHistoryAsync(userId, applicationId, traceId);

            _logger.LogInformation("Completed GetStatusHistory successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<ApplicationStatusHistoryResponse>>.SuccessResponse(result, "Status history retrieved successfully.", StatusCodes.Status200OK));
        }
    }
}