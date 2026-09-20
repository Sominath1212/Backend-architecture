using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Experience;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/candidates/me/[controller]")]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;
        private readonly ILogger<ExperienceController> _logger;

        public ExperienceController(
            IExperienceService experienceService,
            ILogger<ExperienceController> logger)
        {
            _experienceService = experienceService;
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

            _logger.LogInformation("Starting GetAll Experience. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _experienceService.GetAllByUserIdAsync(userId, traceId);

            _logger.LogInformation("Completed GetAll Experience successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<ExperienceResponse>>.SuccessResponse(result, "Experience records retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("{experienceId:int}")]
        public async Task<IActionResult> GetById(int experienceId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetById Experience. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);

            var result = await _experienceService.GetByIdAsync(userId, experienceId, traceId);

            _logger.LogInformation("Completed GetById Experience successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ExperienceResponse>.SuccessResponse(result, "Experience record retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExperienceRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Create Experience. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _experienceService.CreateAsync(userId, request, traceId);

            _logger.LogInformation("Completed Create Experience successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ExperienceResponse>.SuccessResponse(result, "Experience record created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("{experienceId:int}")]
        public async Task<IActionResult> Update(int experienceId, [FromBody] UpdateExperienceRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Update Experience. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);

            var result = await _experienceService.UpdateAsync(userId, experienceId, request, traceId);

            _logger.LogInformation("Completed Update Experience successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<ExperienceResponse>.SuccessResponse(result, "Experience record updated successfully.", StatusCodes.Status200OK));
        }

        [HttpDelete("{experienceId:int}")]
        public async Task<IActionResult> Delete(int experienceId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Experience. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);

            await _experienceService.DeleteAsync(userId, experienceId, traceId);

            _logger.LogInformation("Completed Delete Experience successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Experience record deleted successfully.", StatusCodes.Status200OK));
        }
    }
}