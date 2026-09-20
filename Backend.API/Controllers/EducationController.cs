using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Education;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/candidates/me/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;
        private readonly ILogger<EducationController> _logger;

        public EducationController(
            IEducationService educationService,
            ILogger<EducationController> logger)
        {
            _educationService = educationService;
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

            _logger.LogInformation("Starting GetAll Education. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _educationService.GetAllByUserIdAsync(userId, traceId);

            _logger.LogInformation("Completed GetAll Education successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<EducationResponse>>.SuccessResponse(result, "Education records retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("{educationId:int}")]
        public async Task<IActionResult> GetById(int educationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetById Education. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);

            var result = await _educationService.GetByIdAsync(userId, educationId, traceId);

            _logger.LogInformation("Completed GetById Education successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<EducationResponse>.SuccessResponse(result, "Education record retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEducationRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Create Education. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _educationService.CreateAsync(userId, request, traceId);

            _logger.LogInformation("Completed Create Education successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<EducationResponse>.SuccessResponse(result, "Education record created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("{educationId:int}")]
        public async Task<IActionResult> Update(int educationId, [FromBody] UpdateEducationRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Update Education. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);

            var result = await _educationService.UpdateAsync(userId, educationId, request, traceId);

            _logger.LogInformation("Completed Update Education successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<EducationResponse>.SuccessResponse(result, "Education record updated successfully.", StatusCodes.Status200OK));
        }

        [HttpDelete("{educationId:int}")]
        public async Task<IActionResult> Delete(int educationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Education. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);

            await _educationService.DeleteAsync(userId, educationId, traceId);

            _logger.LogInformation("Completed Delete Education successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Education record deleted successfully.", StatusCodes.Status200OK));
        }
    }
}