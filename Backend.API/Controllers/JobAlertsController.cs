using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.JobAlerts;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/candidates/me/job-alerts")]
    public class JobAlertsController : ControllerBase
    {
        private readonly IJobAlertService _jobAlertService;
        private readonly ILogger<JobAlertsController> _logger;

        public JobAlertsController(IJobAlertService jobAlertService, ILogger<JobAlertsController> logger)
        {
            _jobAlertService = jobAlertService;
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
        public async Task<IActionResult> GetMyAlerts()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMyAlerts. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _jobAlertService.GetMyAlertsAsync(userId, traceId);

            _logger.LogInformation("Completed GetMyAlerts successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<JobAlertResponse>>.SuccessResponse(result, "Job alerts retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("{alertId:int}")]
        public async Task<IActionResult> GetById(int alertId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetById. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

            var result = await _jobAlertService.GetByIdAsync(userId, alertId, traceId);

            _logger.LogInformation("Completed GetById successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<JobAlertResponse>.SuccessResponse(result, "Job alert retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobAlertRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Create Job Alert. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _jobAlertService.CreateAsync(userId, request, traceId);

            _logger.LogInformation("Completed Create Job Alert successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<JobAlertResponse>.SuccessResponse(result, "Job alert created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("{alertId:int}")]
        public async Task<IActionResult> Update(int alertId, [FromBody] UpdateJobAlertRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Update Job Alert. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

            var result = await _jobAlertService.UpdateAsync(userId, alertId, request, traceId);

            _logger.LogInformation("Completed Update Job Alert successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<JobAlertResponse>.SuccessResponse(result, "Job alert updated successfully.", StatusCodes.Status200OK));
        }

        [HttpDelete("{alertId:int}")]
        public async Task<IActionResult> Delete(int alertId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Job Alert. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

            await _jobAlertService.DeleteAsync(userId, alertId, traceId);

            _logger.LogInformation("Completed Delete Job Alert successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Job alert deleted successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("{alertId:int}/enable")]
        public async Task<IActionResult> Enable(int alertId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Enable Job Alert. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

            await _jobAlertService.EnableAsync(userId, alertId, traceId);

            _logger.LogInformation("Completed Enable Job Alert successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Job alert enabled successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("{alertId:int}/disable")]
        public async Task<IActionResult> Disable(int alertId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Disable Job Alert. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

            await _jobAlertService.DisableAsync(userId, alertId, traceId);

            _logger.LogInformation("Completed Disable Job Alert successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Job alert disabled successfully.", StatusCodes.Status200OK));
        }
    }
}