using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Education;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class EducationService : IEducationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EducationService> _logger;

    public EducationService(
        IUnitOfWork unitOfWork,
        ILogger<EducationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<EducationResponse>> GetAllByUserIdAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetAllByUserIdAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var educations = await _unitOfWork.Repository<Education>()
            .FindAsync(e => e.UserId == userId);

        var response = educations.Select(MapToResponse).ToList();

        _logger.LogInformation("GetAllByUserIdAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<EducationResponse> GetByIdAsync(string userId, int educationId, string traceId)
    {
        _logger.LogInformation("Starting GetByIdAsync. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);

        var education = await _unitOfWork.Repository<Education>()
            .FirstOrDefaultAsync(e => e.Id == educationId && e.UserId == userId);

        if (education is null)
        {
            _logger.LogWarning("Education record not found or access denied. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);
            throw new NotFoundException("Education record not found.");
        }

        _logger.LogInformation("GetByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return MapToResponse(education);
    }

    public async Task<EducationResponse> CreateAsync(string userId, CreateEducationRequest request, string traceId)
    {
        _logger.LogInformation("Starting CreateAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        // Business rule: EndYear must be null or >= StartYear if not currently studying
        if (!request.IsCurrentlyStudying && request.EndYear.HasValue && request.EndYear.Value < request.StartYear)
        {
            _logger.LogWarning("Invalid date range. StartYear: {StartYear}, EndYear: {EndYear}, TraceId: {TraceId}", request.StartYear, request.EndYear, traceId);
            throw new BadRequestException("End year must be greater than or equal to start year.");
        }

        var education = new Education
        {
            UserId = userId,
            Degree = request.Degree,
            Institution = request.Institution,
            Specialization = request.Specialization,
            StartYear = request.StartYear,
            EndYear = request.EndYear,
            IsCurrentlyStudying = request.IsCurrentlyStudying,
            Grade = request.Grade
        };

        await _unitOfWork.Repository<Education>().AddAsync(education);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateAsync completed successfully. EducationId: {EducationId}, TraceId: {TraceId}", education.Id, traceId);
        return MapToResponse(education);
    }

    public async Task<EducationResponse> UpdateAsync(string userId, int educationId, UpdateEducationRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateAsync. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);

        var education = await _unitOfWork.Repository<Education>()
            .FirstOrDefaultAsync(e => e.Id == educationId && e.UserId == userId);

        if (education is null)
        {
            _logger.LogWarning("Education record not found for update. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);
            throw new NotFoundException("Education record not found.");
        }

        // Business rule validation for update
        var newStartYear = request.StartYear ?? education.StartYear;
        var newEndYear = request.EndYear ?? education.EndYear;
        var isCurrentlyStudying = request.IsCurrentlyStudying ?? education.IsCurrentlyStudying;

        if (!isCurrentlyStudying && newEndYear.HasValue && newEndYear.Value < newStartYear)
        {
            _logger.LogWarning("Invalid date range on update. StartYear: {StartYear}, EndYear: {EndYear}, TraceId: {TraceId}", newStartYear, newEndYear, traceId);
            throw new BadRequestException("End year must be greater than or equal to start year.");
        }

        // Update only supplied fields
        if (request.Degree is not null) education.Degree = request.Degree;
        if (request.Institution is not null) education.Institution = request.Institution;
        if (request.Specialization is not null) education.Specialization = request.Specialization;
        if (request.StartYear is not null) education.StartYear = request.StartYear.Value;
        if (request.EndYear is not null) education.EndYear = request.EndYear;
        if (request.IsCurrentlyStudying is not null) education.IsCurrentlyStudying = request.IsCurrentlyStudying.Value;
        if (request.Grade is not null) education.Grade = request.Grade;

        _unitOfWork.Repository<Education>().Update(education);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UpdateAsync completed successfully. EducationId: {EducationId}, TraceId: {TraceId}", education.Id, traceId);
        return MapToResponse(education);
    }

    public async Task DeleteAsync(string userId, int educationId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);

        var education = await _unitOfWork.Repository<Education>()
            .FirstOrDefaultAsync(e => e.Id == educationId && e.UserId == userId);

        if (education is null)
        {
            _logger.LogWarning("Education record not found for deletion. UserId: {UserId}, EducationId: {EducationId}, TraceId: {TraceId}", userId, educationId, traceId);
            throw new NotFoundException("Education record not found.");
        }

        _unitOfWork.Repository<Education>().Delete(education);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. EducationId: {EducationId}, TraceId: {TraceId}", educationId, traceId);
    }

    private static EducationResponse MapToResponse(Education education)
    {
        return new EducationResponse
        {
            Id = education.Id,
            UserId = education.UserId,
            Degree = education.Degree,
            Institution = education.Institution,
            Specialization = education.Specialization,
            StartYear = education.StartYear,
            EndYear = education.EndYear,
            IsCurrentlyStudying = education.IsCurrentlyStudying,
            Grade = education.Grade
        };
    }
}