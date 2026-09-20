using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Companies;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(IUnitOfWork unitOfWork, ILogger<CompanyService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CompanyResponse>> GetAllAsync(string traceId)
    {
        _logger.LogInformation("Starting GetAllAsync. TraceId: {TraceId}", traceId);

        var companies = await _unitOfWork.Repository<Company>().GetAllAsync();
        var response = companies.Select(MapToResponse).ToList();

        _logger.LogInformation("GetAllAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<CompanyResponse> GetByIdAsync(int companyId, string traceId)
    {
        _logger.LogInformation("Starting GetByIdAsync. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(companyId);

        if (company is null)
        {
            _logger.LogWarning("Company not found. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);
            throw new NotFoundException("Company not found.");
        }

        _logger.LogInformation("GetByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return MapToResponse(company);
    }

    public async Task<CompanyResponse> CreateAsync(string userId, CreateCompanyRequest request, string traceId)
    {
        _logger.LogInformation("Starting CreateAsync. UserId: {UserId}, CompanyName: {CompanyName}, TraceId: {TraceId}", userId, request.Name, traceId);

        var company = new Company
        {
            Name = request.Name,
            Website = request.Website,
            Industry = request.Industry,
            CompanySize = request.CompanySize,
            FoundedYear = request.FoundedYear,
            Headquarters = request.Headquarters,
            Description = request.Description,
            VerificationStatus = Domain.Enums.CompanyVerificationStatus.Pending
        };

        await _unitOfWork.Repository<Company>().AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateAsync completed successfully. CompanyId: {CompanyId}, TraceId: {TraceId}", company.Id, traceId);
        return MapToResponse(company);
    }

    public async Task<CompanyResponse> UpdateAsync(string userId, int companyId, UpdateCompanyRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateAsync. UserId: {UserId}, CompanyId: {CompanyId}, TraceId: {TraceId}", userId, companyId, traceId);

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(companyId);

        if (company is null)
        {
            _logger.LogWarning("Company not found for update. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);
            throw new NotFoundException("Company not found.");
        }

        // TODO: Add authorization check to ensure user is admin of this company

        if (request.Name is not null) company.Name = request.Name;
        if (request.Website is not null) company.Website = request.Website;
        if (request.Industry is not null) company.Industry = request.Industry;
        if (request.CompanySize is not null) company.CompanySize = request.CompanySize;
        if (request.FoundedYear is not null) company.FoundedYear = request.FoundedYear;
        if (request.Headquarters is not null) company.Headquarters = request.Headquarters;
        if (request.Description is not null) company.Description = request.Description;

        _unitOfWork.Repository<Company>().Update(company);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UpdateAsync completed successfully. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);
        return MapToResponse(company);
    }

    public async Task DeleteAsync(string userId, int companyId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, CompanyId: {CompanyId}, TraceId: {TraceId}", userId, companyId, traceId);

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(companyId);

        if (company is null)
        {
            _logger.LogWarning("Company not found for deletion. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);
            throw new NotFoundException("Company not found.");
        }

        // TODO: Add authorization check to ensure user is admin of this company

        _unitOfWork.Repository<Company>().Delete(company);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. CompanyId: {CompanyId}, TraceId: {TraceId}", companyId, traceId);
    }

    private static CompanyResponse MapToResponse(Company company)
    {
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Website = company.Website,
            Industry = company.Industry,
            CompanySize = company.CompanySize,
            FoundedYear = company.FoundedYear,
            Headquarters = company.Headquarters,
            Description = company.Description,
            LogoPath = company.LogoPath,
            CoverImagePath = company.CoverImagePath,
            VerificationStatus = company.VerificationStatus,
            CreatedAt = company.CreatedAt
        };
    }
}