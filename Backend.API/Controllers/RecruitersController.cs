using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Recruiters;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RecruitersController : ControllerBase
    {
        private readonly IRecruiterService _recruiterService;
        private readonly ILogger<RecruitersController> _logger;

        public RecruitersController(IRecruiterService recruiterService, ILogger<RecruitersController> logger)
        {
            _recruiterService = recruiterService;
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

        [HttpGet("me")]
        public async Task<IActionResult> GetMyRecruiterProfiles()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMyRecruiterProfiles. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _recruiterService.GetByUserIdAsync(userId, traceId);

            _logger.LogInformation("Completed GetMyRecruiterProfiles successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<RecruiterProfileResponse>>.SuccessResponse(result, "Recruiter profiles retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("company/{companyId:int}")]
        public async Task<IActionResult> GetByCompanyId(int companyId)
        {
            var traceId = GetTraceId();
            _logger.LogInformation("Starting GetByCompanyId. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);

            var result = await _recruiterService.GetByCompanyIdAsync(companyId, traceId);

            _logger.LogInformation("Completed GetByCompanyId successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<RecruiterProfileResponse>>.SuccessResponse(result, "Recruiters retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecruiterProfileRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Create Recruiter. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _recruiterService.CreateAsync(userId, request, traceId);

            _logger.LogInformation("Completed Create Recruiter successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<RecruiterProfileResponse>.SuccessResponse(result, "Recruiter profile created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("{recruiterId:int}")]
        public async Task<IActionResult> Update(int recruiterId, [FromBody] UpdateRecruiterProfileRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Update Recruiter. UserId: {UserId}, RecruiterId: {RecruiterId}, TraceId: {TraceId}", userId, recruiterId, traceId);

            var result = await _recruiterService.UpdateAsync(userId, recruiterId, request, traceId);

            _logger.LogInformation("Completed Update Recruiter successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<RecruiterProfileResponse>.SuccessResponse(result, "Recruiter profile updated successfully.", StatusCodes.Status200OK));
        }

        [HttpDelete("{recruiterId:int}")]
        public async Task<IActionResult> Delete(int recruiterId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Recruiter. UserId: {UserId}, RecruiterId: {RecruiterId}, TraceId: {TraceId}", userId, recruiterId, traceId);

            await _recruiterService.DeleteAsync(userId, recruiterId, traceId);

            _logger.LogInformation("Completed Delete Recruiter successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Recruiter profile deleted successfully.", StatusCodes.Status200OK));
        }
    }
}