using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.JobAlerts;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class JobAlertService : IJobAlertService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobAlertService> _logger;

    public JobAlertService(IUnitOfWork unitOfWork, ILogger<JobAlertService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<JobAlertResponse>> GetMyAlertsAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetMyAlertsAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var alerts = await _unitOfWork.Repository<JobAlert>()
            .FindAsync(a => a.UserId == userId);

        var response = alerts.Select(MapToResponse).ToList();

        _logger.LogInformation("GetMyAlertsAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<JobAlertResponse> GetByIdAsync(string userId, int alertId, string traceId)
    {
        _logger.LogInformation("Starting GetByIdAsync. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

        var alert = await _unitOfWork.Repository<JobAlert>()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

        if (alert is null)
        {
            _logger.LogWarning("Job alert not found or access denied. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
            throw new NotFoundException("Job alert not found.");
        }

        _logger.LogInformation("GetByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return MapToResponse(alert);
    }

    public async Task<JobAlertResponse> CreateAsync(string userId, CreateJobAlertRequest request, string traceId)
    {
        _logger.LogInformation("Starting CreateAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        // Validate at least one filter is provided
        if (string.IsNullOrWhiteSpace(request.Keyword) &&
            string.IsNullOrWhiteSpace(request.Location) &&
            !request.ExperienceMin.HasValue &&
            !request.ExperienceMax.HasValue &&
            !request.SalaryMin.HasValue &&
            !request.SalaryMax.HasValue &&
            !request.WorkMode.HasValue &&
            !request.EmploymentType.HasValue)
        {
            _logger.LogWarning("No filters provided for job alert. TraceId: {TraceId}", traceId);
            throw new BadRequestException("At least one filter criteria must be provided for a job alert.");
        }

        // Validate ranges
        if (request.ExperienceMin.HasValue && request.ExperienceMax.HasValue && request.ExperienceMin > request.ExperienceMax)
        {
            throw new BadRequestException("Minimum experience cannot be greater than maximum experience.");
        }

        if (request.SalaryMin.HasValue && request.SalaryMax.HasValue && request.SalaryMin > request.SalaryMax)
        {
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary.");
        }

        var alert = new JobAlert
        {
            UserId = userId,
            Keyword = request.Keyword,
            Location = request.Location,
            ExperienceMin = request.ExperienceMin,
            ExperienceMax = request.ExperienceMax,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            WorkMode = request.WorkMode,
            EmploymentType = request.EmploymentType,
            IsActive = request.IsActive
        };

        await _unitOfWork.Repository<JobAlert>().AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateAsync completed successfully. AlertId: {AlertId}, TraceId: {TraceId}", alert.Id, traceId);
        return MapToResponse(alert);
    }

    public async Task<JobAlertResponse> UpdateAsync(string userId, int alertId, UpdateJobAlertRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateAsync. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

        var alert = await _unitOfWork.Repository<JobAlert>()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

        if (alert is null)
        {
            _logger.LogWarning("Job alert not found for update. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
            throw new NotFoundException("Job alert not found.");
        }

        // Validate ranges
        var newExpMin = request.ExperienceMin ?? alert.ExperienceMin;
        var newExpMax = request.ExperienceMax ?? alert.ExperienceMax;
        if (newExpMin.HasValue && newExpMax.HasValue && newExpMin > newExpMax)
        {
            throw new BadRequestException("Minimum experience cannot be greater than maximum experience.");
        }

        var newSalaryMin = request.SalaryMin ?? alert.SalaryMin;
        var newSalaryMax = request.SalaryMax ?? alert.SalaryMax;
        if (newSalaryMin.HasValue && newSalaryMax.HasValue && newSalaryMin > newSalaryMax)
        {
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary.");
        }

        // Update only supplied fields
        if (request.Keyword is not null) alert.Keyword = request.Keyword;
        if (request.Location is not null) alert.Location = request.Location;
        if (request.ExperienceMin is not null) alert.ExperienceMin = request.ExperienceMin;
        if (request.ExperienceMax is not null) alert.ExperienceMax = request.ExperienceMax;
        if (request.SalaryMin is not null) alert.SalaryMin = request.SalaryMin;
        if (request.SalaryMax is not null) alert.SalaryMax = request.SalaryMax;
        if (request.WorkMode is not null) alert.WorkMode = request.WorkMode;
        if (request.EmploymentType is not null) alert.EmploymentType = request.EmploymentType;
        if (request.IsActive is not null) alert.IsActive = request.IsActive.Value;

        _unitOfWork.Repository<JobAlert>().Update(alert);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UpdateAsync completed successfully. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
        return MapToResponse(alert);
    }

    public async Task DeleteAsync(string userId, int alertId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

        var alert = await _unitOfWork.Repository<JobAlert>()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

        if (alert is null)
        {
            _logger.LogWarning("Job alert not found for deletion. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
            throw new NotFoundException("Job alert not found.");
        }

        _unitOfWork.Repository<JobAlert>().Delete(alert);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
    }

    public async Task EnableAsync(string userId, int alertId, string traceId)
    {
        _logger.LogInformation("Starting EnableAsync. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

        var alert = await _unitOfWork.Repository<JobAlert>()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

        if (alert is null)
        {
            _logger.LogWarning("Job alert not found. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
            throw new NotFoundException("Job alert not found.");
        }

        alert.IsActive = true;
        _unitOfWork.Repository<JobAlert>().Update(alert);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("EnableAsync completed successfully. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
    }

    public async Task DisableAsync(string userId, int alertId, string traceId)
    {
        _logger.LogInformation("Starting DisableAsync. UserId: {UserId}, AlertId: {AlertId}, TraceId: {TraceId}", userId, alertId, traceId);

        var alert = await _unitOfWork.Repository<JobAlert>()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

        if (alert is null)
        {
            _logger.LogWarning("Job alert not found. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
            throw new NotFoundException("Job alert not found.");
        }

        alert.IsActive = false;
        _unitOfWork.Repository<JobAlert>().Update(alert);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DisableAsync completed successfully. AlertId: {AlertId}, TraceId: {TraceId}", alertId, traceId);
    }

    private static JobAlertResponse MapToResponse(JobAlert alert)
    {
        return new JobAlertResponse
        {
            Id = alert.Id,
            Keyword = alert.Keyword,
            Location = alert.Location,
            ExperienceMin = alert.ExperienceMin,
            ExperienceMax = alert.ExperienceMax,
            SalaryMin = alert.SalaryMin,
            SalaryMax = alert.SalaryMax,
            WorkMode = alert.WorkMode,
            EmploymentType = alert.EmploymentType,
            IsActive = alert.IsActive,
            LastCheckedAt = alert.LastCheckedAt,
            CreatedAt = alert.CreatedAt
        };
    }
}