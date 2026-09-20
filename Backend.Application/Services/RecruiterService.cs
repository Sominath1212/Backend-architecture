using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Recruiters;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class RecruiterService : IRecruiterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecruiterService> _logger;

    public RecruiterService(IUnitOfWork unitOfWork, ILogger<RecruiterService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RecruiterProfileResponse>> GetByUserIdAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetByUserIdAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var recruiters = await _unitOfWork.Repository<RecruiterProfile>()
            .FindAsync(r => r.UserId == userId);

        var companyIds = recruiters.Select(r => r.CompanyId).Distinct().ToList();
        var companies = await _unitOfWork.Repository<Company>().FindAsync(c => companyIds.Contains(c.Id));
        var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

        var response = recruiters.Select(r => new RecruiterProfileResponse
        {
            Id = r.Id,
            UserId = r.UserId,
            CompanyId = r.CompanyId,
            CompanyName = companyDict.TryGetValue(r.CompanyId, out var name) ? name : "Unknown",
            Designation = r.Designation,
            Department = r.Department,
            Phone = r.Phone
        }).ToList();

        _logger.LogInformation("GetByUserIdAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<IReadOnlyList<RecruiterProfileResponse>> GetByCompanyIdAsync(int companyId, string traceId)
    {
        _logger.LogInformation("Starting GetByCompanyIdAsync. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);

        var recruiters = await _unitOfWork.Repository<RecruiterProfile>()
            .FindAsync(r => r.CompanyId == companyId);

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(companyId);
        var companyName = company?.Name ?? "Unknown";

        var response = recruiters.Select(r => new RecruiterProfileResponse
        {
            Id = r.Id,
            UserId = r.UserId,
            CompanyId = r.CompanyId,
            CompanyName = companyName,
            Designation = r.Designation,
            Department = r.Department,
            Phone = r.Phone
        }).ToList();

        _logger.LogInformation("GetByCompanyIdAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<RecruiterProfileResponse> CreateAsync(string userId, CreateRecruiterProfileRequest request, string traceId)
    {
        _logger.LogInformation("Starting CreateAsync. UserId: {UserId}, CompanyId: {CompanyId}, TraceId: {TraceId}", userId, request.CompanyId, traceId);

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(request.CompanyId);
        if (company is null)
        {
            _logger.LogWarning("Company not found. CompanyId: {CompanyId}, TraceId: {TraceId}", request.CompanyId, traceId);
            throw new NotFoundException("Company not found.");
        }

        var existing = await _unitOfWork.Repository<RecruiterProfile>()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CompanyId == request.CompanyId);

        if (existing is not null)
        {
            _logger.LogWarning("Recruiter profile already exists. UserId: {UserId}, CompanyId: {CompanyId}, TraceId: {TraceId}", userId, request.CompanyId, traceId);
            throw new BadRequestException("You are already a recruiter for this company.");
        }

        var recruiter = new RecruiterProfile
        {
            UserId = userId,
            CompanyId = request.CompanyId,
            Designation = request.Designation,
            Department = request.Department,
            Phone = request.Phone
        };

        await _unitOfWork.Repository<RecruiterProfile>().AddAsync(recruiter);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateAsync completed successfully. RecruiterId: {RecruiterId}, TraceId: {TraceId}", recruiter.Id, traceId);

        return new RecruiterProfileResponse
        {
            Id = recruiter.Id,
            UserId = recruiter.UserId,
            CompanyId = recruiter.CompanyId,
            CompanyName = company.Name,
            Designation = recruiter.Designation,
            Department = recruiter.Department,
            Phone = recruiter.Phone
        };
    }

    public async Task<RecruiterProfileResponse> UpdateAsync(string userId, int recruiterId, UpdateRecruiterProfileRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateAsync. UserId: {UserId}, RecruiterId: {RecruiterId}, TraceId: {TraceId}", userId, recruiterId, traceId);

        var recruiter = await _unitOfWork.Repository<RecruiterProfile>()
            .FirstOrDefaultAsync(r => r.Id == recruiterId && r.UserId == userId);

        if (recruiter is null)
        {
            _logger.LogWarning("Recruiter profile not found. UserId: {UserId}, RecruiterId: {RecruiterId}, TraceId: {TraceId}", userId, recruiterId, traceId);
            throw new NotFoundException("Recruiter profile not found.");
        }

        if (request.Designation is not null) recruiter.Designation = request.Designation;
        if (request.Department is not null) recruiter.Department = request.Department;
        if (request.Phone is not null) recruiter.Phone = request.Phone;

        _unitOfWork.Repository<RecruiterProfile>().Update(recruiter);
        await _unitOfWork.SaveChangesAsync();

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(recruiter.CompanyId);

        _logger.LogInformation("UpdateAsync completed successfully. RecruiterId: {RecruiterId}, TraceId: {TraceId}", recruiterId, traceId);

        return new RecruiterProfileResponse
        {
            Id = recruiter.Id,
            UserId = recruiter.UserId,
            CompanyId = recruiter.CompanyId,
            CompanyName = company?.Name ?? "Unknown",
            Designation = recruiter.Designation,
            Department = recruiter.Department,
            Phone = recruiter.Phone
        };
    }

    public async Task DeleteAsync(string userId, int recruiterId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, RecruiterId: {RecruiterId}, TraceId: {TraceId}", userId, recruiterId, traceId);

        var recruiter = await _unitOfWork.Repository<RecruiterProfile>()
            .FirstOrDefaultAsync(r => r.Id == recruiterId && r.UserId == userId);

        if (recruiter is null)
        {
            _logger.LogWarning("Recruiter profile not found for deletion. UserId: {UserId}, RecruiterId: {RecruiterId}, TraceId: {TraceId}", userId, recruiterId, traceId);
            throw new NotFoundException("Recruiter profile not found.");
        }

        _unitOfWork.Repository<RecruiterProfile>().Delete(recruiter);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. RecruiterId: {RecruiterId}, TraceId: {TraceId}", recruiterId, traceId);
    }
}