using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Skills;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CandidateSkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;
        private readonly ILogger<CandidateSkillsController> _logger;

        public CandidateSkillsController(ISkillService skillService, ILogger<CandidateSkillsController> logger)
        {
            _skillService = skillService;
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

        // Master Skills (Public/Authenticated)
        [AllowAnonymous]
        [HttpGet("skills")]
        public async Task<IActionResult> GetAllMasterSkills()
        {
            var traceId = GetTraceId();
            _logger.LogInformation("Starting GetAllMasterSkills. TraceId: {TraceId}", traceId);

            var result = await _skillService.GetAllMasterSkillsAsync(traceId);

            _logger.LogInformation("Completed GetAllMasterSkills successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<SkillResponse>>.SuccessResponse(result, "Master skills retrieved successfully.", StatusCodes.Status200OK));
        }

        // Candidate Skills
        [Authorize]
        [HttpGet("candidates/me/skills")]
        public async Task<IActionResult> GetMySkills()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMySkills. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _skillService.GetCandidateSkillsAsync(userId, traceId);

            _logger.LogInformation("Completed GetMySkills successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<CandidateSkillResponse>>.SuccessResponse(result, "Candidate skills retrieved successfully.", StatusCodes.Status200OK));
        }

        [Authorize]
        [HttpPost("candidates/me/skills")]
        public async Task<IActionResult> CreateMySkill([FromBody] CreateCandidateSkillRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting CreateMySkill. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _skillService.CreateCandidateSkillAsync(userId, request, traceId);

            _logger.LogInformation("Completed CreateMySkill successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<CandidateSkillResponse>.SuccessResponse(result, "Skill added successfully.", StatusCodes.Status201Created));
        }

        [Authorize]
        [HttpPut("candidates/me/skills/{candidateSkillId:int}")]
        public async Task<IActionResult> UpdateMySkill(int candidateSkillId, [FromBody] UpdateCandidateSkillRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting UpdateMySkill. UserId: {UserId}, CandidateSkillId: {Id}, TraceId: {TraceId}", userId, candidateSkillId, traceId);

            var result = await _skillService.UpdateCandidateSkillAsync(userId, candidateSkillId, request, traceId);

            _logger.LogInformation("Completed UpdateMySkill successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<CandidateSkillResponse>.SuccessResponse(result, "Skill updated successfully.", StatusCodes.Status200OK));
        }

        [Authorize]
        [HttpDelete("candidates/me/skills/{candidateSkillId:int}")]
        public async Task<IActionResult> DeleteMySkill(int candidateSkillId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting DeleteMySkill. UserId: {UserId}, CandidateSkillId: {Id}, TraceId: {TraceId}", userId, candidateSkillId, traceId);

            await _skillService.DeleteCandidateSkillAsync(userId, candidateSkillId, traceId);

            _logger.LogInformation("Completed DeleteMySkill successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Skill removed successfully.", StatusCodes.Status200OK));
        }
    }
}