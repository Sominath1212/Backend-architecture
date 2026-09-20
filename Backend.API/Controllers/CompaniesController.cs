using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Companies;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var traceId = GetTraceId();
            _logger.LogInformation("Starting GetAll Companies. TraceId: {TraceId}", traceId);

            var result = await _companyService.GetAllAsync(traceId);

            _logger.LogInformation("Completed GetAll Companies successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<IReadOnlyList<CompanyResponse>>.SuccessResponse(result, "Companies retrieved successfully.", StatusCodes.Status200OK));
        }

        [AllowAnonymous]
        [HttpGet("{companyId:int}")]
        public async Task<IActionResult> GetById(int companyId)
        {
            var traceId = GetTraceId();
            _logger.LogInformation("Starting GetById Company. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);

            var result = await _companyService.GetByIdAsync(companyId, traceId);

            _logger.LogInformation("Completed GetById Company successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<CompanyResponse>.SuccessResponse(result, "Company retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Create Company. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _companyService.CreateAsync(userId, request, traceId);

            _logger.LogInformation("Completed Create Company successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<CompanyResponse>.SuccessResponse(result, "Company created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("{companyId:int}")]
        public async Task<IActionResult> Update(int companyId, [FromBody] UpdateCompanyRequest request)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Update Company. UserId: {UserId}, CompanyId: {CompanyId}, TraceId: {TraceId}", userId, companyId, traceId);

            var result = await _companyService.UpdateAsync(userId, companyId, request, traceId);

            _logger.LogInformation("Completed Update Company successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<CompanyResponse>.SuccessResponse(result, "Company updated successfully.", StatusCodes.Status200OK));
        }

        [HttpDelete("{companyId:int}")]
        public async Task<IActionResult> Delete(int companyId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Company. UserId: {UserId}, CompanyId: {CompanyId}, TraceId: {TraceId}", userId, companyId, traceId);

            await _companyService.DeleteAsync(userId, companyId, traceId);

            _logger.LogInformation("Completed Delete Company successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Company deleted successfully.", StatusCodes.Status200OK));
        }
    }
}