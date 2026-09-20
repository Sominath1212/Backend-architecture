using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.CandidateProfile;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CandidateProfilesController : ControllerBase
    {
        private readonly ICandidateProfileService _candidateProfileService;
        private readonly ILogger<CandidateProfilesController> _logger;

        public CandidateProfilesController(
            ICandidateProfileService candidateProfileService,
            ILogger<CandidateProfilesController> logger)
        {
            _candidateProfileService = candidateProfileService;
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

        // GET: api/v1/candidateprofiles/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMyProfile. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);

            try
            {
                var result = await _candidateProfileService.GetProfileByUserIdAsync(userId, traceId);

                _logger.LogInformation("Completed GetMyProfile successfully. TraceId: {TraceId}", traceId);

                return Ok(ApiResponse<CandidateProfileResponse>.SuccessResponse(
                    result,
                    "Profile retrieved successfully.",
                    StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get profile. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw; // Let global exception handler manage the response
            }
        }

        // POST: api/v1/candidateprofiles/me
        [HttpPost("me")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateCandidateProfileRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting CreateProfile. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);

            try
            {
                var result = await _candidateProfileService.CreateProfileAsync(userId, request, traceId);

                _logger.LogInformation("Completed CreateProfile successfully. TraceId: {TraceId}", traceId);

                return Ok(ApiResponse<CandidateProfileResponse>.SuccessResponse(
                    result,
                    "Profile created successfully.",
                    StatusCodes.Status201Created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create profile. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw;
            }
        }

        // PUT: api/v1/candidateprofiles/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCandidateProfileRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting UpdateProfile. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);

            try
            {
                var result = await _candidateProfileService.UpdateProfileAsync(userId, request, traceId);

                _logger.LogInformation("Completed UpdateProfile successfully. TraceId: {TraceId}", traceId);

                return Ok(ApiResponse<CandidateProfileResponse>.SuccessResponse(
                    result,
                    "Profile updated successfully.",
                    StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw;
            }
        }
    }
}