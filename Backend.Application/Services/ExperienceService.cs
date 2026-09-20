using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Experience;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class ExperienceService : IExperienceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExperienceService> _logger;

    public ExperienceService(IUnitOfWork unitOfWork, ILogger<ExperienceService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ExperienceResponse>> GetAllByUserIdAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetAllByUserIdAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var experiences = await _unitOfWork.Repository<Experience>()
            .FindAsync(e => e.UserId == userId);

        var response = experiences.Select(MapToResponse).ToList();

        _logger.LogInformation("GetAllByUserIdAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<ExperienceResponse> GetByIdAsync(string userId, int experienceId, string traceId)
    {
        _logger.LogInformation("Starting GetByIdAsync. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);

        var experience = await _unitOfWork.Repository<Experience>()
            .FirstOrDefaultAsync(e => e.Id == experienceId && e.UserId == userId);

        if (experience is null)
        {
            _logger.LogWarning("Experience record not found or access denied. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);
            throw new NotFoundException("Experience record not found.");
        }

        _logger.LogInformation("GetByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return MapToResponse(experience);
    }

    public async Task<ExperienceResponse> CreateAsync(string userId, CreateExperienceRequest request, string traceId)
    {
        _logger.LogInformation("Starting CreateAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        // Business rule: EndDate must be null or >= StartDate if not currently working
        if (!request.IsCurrentlyWorking && request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
        {
            _logger.LogWarning("Invalid date range. StartDate: {StartDate}, EndDate: {EndDate}, TraceId: {TraceId}", request.StartDate, request.EndDate, traceId);
            throw new BadRequestException("End date must be greater than or equal to start date.");
        }

        var experience = new Experience
        {
            UserId = userId,
            Company = request.Company,
            Designation = request.Designation,
            EmploymentType = request.EmploymentType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrentlyWorking = request.IsCurrentlyWorking,
            Location = request.Location,
            Description = request.Description,
            SkillsUsed = request.SkillsUsed
        };

        await _unitOfWork.Repository<Experience>().AddAsync(experience);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateAsync completed successfully. ExperienceId: {ExperienceId}, TraceId: {TraceId}", experience.Id, traceId);
        return MapToResponse(experience);
    }

    public async Task<ExperienceResponse> UpdateAsync(string userId, int experienceId, UpdateExperienceRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateAsync. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);

        var experience = await _unitOfWork.Repository<Experience>()
            .FirstOrDefaultAsync(e => e.Id == experienceId && e.UserId == userId);

        if (experience is null)
        {
            _logger.LogWarning("Experience record not found for update. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);
            throw new NotFoundException("Experience record not found.");
        }

        // Business rule validation for update
        var newStartDate = request.StartDate ?? experience.StartDate;
        var newEndDate = request.EndDate ?? experience.EndDate;
        var isCurrentlyWorking = request.IsCurrentlyWorking ?? experience.IsCurrentlyWorking;

        if (!isCurrentlyWorking && newEndDate.HasValue && newEndDate.Value < newStartDate)
        {
            _logger.LogWarning("Invalid date range on update. StartDate: {StartDate}, EndDate: {EndDate}, TraceId: {TraceId}", newStartDate, newEndDate, traceId);
            throw new BadRequestException("End date must be greater than or equal to start date.");
        }

        // Update only supplied fields
        if (request.Company is not null) experience.Company = request.Company;
        if (request.Designation is not null) experience.Designation = request.Designation;
        if (request.EmploymentType is not null) experience.EmploymentType = request.EmploymentType.Value;
        if (request.StartDate is not null) experience.StartDate = request.StartDate.Value;
        if (request.EndDate is not null) experience.EndDate = request.EndDate;
        if (request.IsCurrentlyWorking is not null) experience.IsCurrentlyWorking = request.IsCurrentlyWorking.Value;
        if (request.Location is not null) experience.Location = request.Location;
        if (request.Description is not null) experience.Description = request.Description;
        if (request.SkillsUsed is not null) experience.SkillsUsed = request.SkillsUsed;

        _unitOfWork.Repository<Experience>().Update(experience);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UpdateAsync completed successfully. ExperienceId: {ExperienceId}, TraceId: {TraceId}", experience.Id, traceId);
        return MapToResponse(experience);
    }

    public async Task DeleteAsync(string userId, int experienceId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);

        var experience = await _unitOfWork.Repository<Experience>()
            .FirstOrDefaultAsync(e => e.Id == experienceId && e.UserId == userId);

        if (experience is null)
        {
            _logger.LogWarning("Experience record not found for deletion. UserId: {UserId}, ExperienceId: {ExperienceId}, TraceId: {TraceId}", userId, experienceId, traceId);
            throw new NotFoundException("Experience record not found.");
        }

        _unitOfWork.Repository<Experience>().Delete(experience);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. ExperienceId: {ExperienceId}, TraceId: {TraceId}", experienceId, traceId);
    }

    private static ExperienceResponse MapToResponse(Experience experience)
    {
        return new ExperienceResponse
        {
            Id = experience.Id,
            UserId = experience.UserId,
            Company = experience.Company,
            Designation = experience.Designation,
            EmploymentType = experience.EmploymentType,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrentlyWorking = experience.IsCurrentlyWorking,
            Location = experience.Location,
            Description = experience.Description,
            SkillsUsed = experience.SkillsUsed
        };
    }
}