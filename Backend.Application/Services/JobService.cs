using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Jobs;
using Backend.Application.DTOs.Skills;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobService> _logger;

    public JobService(IUnitOfWork unitOfWork, ILogger<JobService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<JobResponse>> SearchJobsAsync(JobSearchRequest request, string traceId)
    {
        _logger.LogInformation(
            "Starting SearchJobsAsync. Keyword: {Keyword}, Location: {Location}, Page: {Page}, TraceId: {TraceId}",
            request.Keyword, request.Location, request.PageNumber, traceId);

        // Fetch all published jobs (basic filtering in memory for now; 
        // for production scale, move to dedicated search engine)
        var allJobs = await _unitOfWork.Repository<Job>()
            .FindAsync(j => j.Status == JobStatus.Published);

        // Apply filters
        var filtered = allJobs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.ToLowerInvariant();
            filtered = filtered.Where(j =>
                j.Title.ToLower().Contains(keyword) ||
                (j.Description != null && j.Description.ToLower().Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var location = request.Location.ToLowerInvariant();
            filtered = filtered.Where(j =>
                j.Location != null && j.Location.ToLower().Contains(location));
        }

        if (request.ExperienceMin.HasValue)
            filtered = filtered.Where(j => j.ExperienceMax == null || j.ExperienceMax >= request.ExperienceMin.Value);

        if (request.ExperienceMax.HasValue)
            filtered = filtered.Where(j => j.ExperienceMin == null || j.ExperienceMin <= request.ExperienceMax.Value);

        if (request.SalaryMin.HasValue)
            filtered = filtered.Where(j => j.SalaryMax == null || j.SalaryMax >= request.SalaryMin.Value);

        if (request.SalaryMax.HasValue)
            filtered = filtered.Where(j => j.SalaryMin == null || j.SalaryMin <= request.SalaryMax.Value);

        if (request.WorkMode.HasValue)
            filtered = filtered.Where(j => j.WorkMode == request.WorkMode.Value);

        if (request.EmploymentType.HasValue)
            filtered = filtered.Where(j => j.EmploymentType == request.EmploymentType.Value);

        if (request.CompanyId.HasValue)
            filtered = filtered.Where(j => j.CompanyId == request.CompanyId.Value);

        var totalCount = filtered.Count();

        var pagedJobs = filtered
            .OrderByDescending(j => j.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Fetch company names and skills for the paged results
        var companyIds = pagedJobs.Select(j => j.CompanyId).Distinct().ToList();
        var companies = await _unitOfWork.Repository<Company>().FindAsync(c => companyIds.Contains(c.Id));
        var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

        var jobIds = pagedJobs.Select(j => j.Id).ToList();
        var jobSkills = await _unitOfWork.Repository<JobSkill>().FindAsync(js => jobIds.Contains(js.JobId));
        var skillIds = jobSkills.Select(js => js.SkillId).Distinct().ToList();
        var skills = await _unitOfWork.Repository<Skill>().FindAsync(s => skillIds.Contains(s.Id));
        var skillDict = skills.ToDictionary(s => s.Id, s => s.Name);

        var jobSkillLookup = jobSkills
            .GroupBy(js => js.JobId)
            .ToDictionary(g => g.Key, g => g.Select(js => js.SkillId).ToList());

        var items = pagedJobs.Select(j => new JobResponse
        {
            Id = j.Id,
            Title = j.Title,
            CompanyId = j.CompanyId,
            CompanyName = companyDict.TryGetValue(j.CompanyId, out var name) ? name : "Unknown",
            Department = j.Department,
            EmploymentType = j.EmploymentType,
            ExperienceMin = j.ExperienceMin,
            ExperienceMax = j.ExperienceMax,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            Location = j.Location,
            WorkMode = j.WorkMode,
            NumberOfOpenings = j.NumberOfOpenings,
            Description = j.Description,
            Requirements = j.Requirements,
            Benefits = j.Benefits,
            ApplicationDeadline = j.ApplicationDeadline,
            Status = j.Status,
            IsFeatured = j.IsFeatured,
            Skills = jobSkillLookup.TryGetValue(j.Id, out var sIds)
                ? sIds.Select(id => new SkillResponse
                {
                    Id = id,
                    Name = skillDict.TryGetValue(id, out var sName) ? sName : "Unknown"
                }).ToList()
                : new List<SkillResponse>(),
            CreatedAt = j.CreatedAt
        }).ToList();

        _logger.LogInformation(
            "SearchJobsAsync completed successfully. TotalCount: {TotalCount}, Returned: {Returned}, TraceId: {TraceId}",
            totalCount, items.Count, traceId);

        return new PagedResult<JobResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<JobResponse> GetByIdAsync(int jobId, string traceId)
    {
        _logger.LogInformation("Starting GetByIdAsync. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);

        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(jobId);

        if (job is null)
        {
            _logger.LogWarning("Job not found. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        var response = await MapToResponseAsync(job);

        _logger.LogInformation("GetByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return response;
    }

    public async Task<JobResponse> CreateAsync(string userId, CreateJobRequest request, string traceId)
    {
        _logger.LogInformation(
            "Starting CreateAsync. UserId: {UserId}, Title: {Title}, CompanyId: {CompanyId}, TraceId: {TraceId}",
            userId, request.Title, request.CompanyId, traceId);

        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(request.CompanyId);
        if (company is null)
        {
            _logger.LogWarning("Company not found. CompanyId: {CompanyId}, TraceId: {TraceId}", request.CompanyId, traceId);
            throw new NotFoundException("Company not found.");
        }

        if (request.SalaryMin.HasValue && request.SalaryMax.HasValue && request.SalaryMin > request.SalaryMax)
        {
            _logger.LogWarning("Invalid salary range. TraceId: {TraceId}", traceId);
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary.");
        }

        if (request.ExperienceMin.HasValue && request.ExperienceMax.HasValue && request.ExperienceMin > request.ExperienceMax)
        {
            _logger.LogWarning("Invalid experience range. TraceId: {TraceId}", traceId);
            throw new BadRequestException("Minimum experience cannot be greater than maximum experience.");
        }

        var job = new Job
        {
            Title = request.Title,
            CompanyId = request.CompanyId,
            PostedByUserId = userId,
            Department = request.Department,
            EmploymentType = request.EmploymentType,
            ExperienceMin = request.ExperienceMin,
            ExperienceMax = request.ExperienceMax,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            Location = request.Location,
            WorkMode = request.WorkMode,
            NumberOfOpenings = request.NumberOfOpenings,
            Description = request.Description,
            Requirements = request.Requirements,
            Benefits = request.Benefits,
            ApplicationDeadline = request.ApplicationDeadline,
            Status = JobStatus.Draft
        };

        await _unitOfWork.Repository<Job>().AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        // Add skills if provided
        if (request.SkillIds is not null && request.SkillIds.Any())
        {
            var jobSkills = request.SkillIds.Select(skillId => new JobSkill
            {
                JobId = job.Id,
                SkillId = skillId
            }).ToList();

            await _unitOfWork.Repository<JobSkill>().AddRangeAsync(jobSkills);
            await _unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("CreateAsync completed successfully. JobId: {JobId}, TraceId: {TraceId}", job.Id, traceId);
        return await MapToResponseAsync(job);
    }

    public async Task<JobResponse> UpdateAsync(string userId, int jobId, UpdateJobRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateAsync. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(jobId);

        if (job is null)
        {
            _logger.LogWarning("Job not found for update. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        if (job.PostedByUserId != userId)
        {
            _logger.LogWarning("Unauthorized update attempt. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);
            throw new ForbiddenException("You are not authorized to update this job.");
        }

        // Validate ranges
        var newSalaryMin = request.SalaryMin ?? job.SalaryMin;
        var newSalaryMax = request.SalaryMax ?? job.SalaryMax;
        if (newSalaryMin.HasValue && newSalaryMax.HasValue && newSalaryMin > newSalaryMax)
        {
            throw new BadRequestException("Minimum salary cannot be greater than maximum salary.");
        }

        var newExpMin = request.ExperienceMin ?? job.ExperienceMin;
        var newExpMax = request.ExperienceMax ?? job.ExperienceMax;
        if (newExpMin.HasValue && newExpMax.HasValue && newExpMin > newExpMax)
        {
            throw new BadRequestException("Minimum experience cannot be greater than maximum experience.");
        }

        // Update only supplied fields
        if (request.Title is not null) job.Title = request.Title;
        if (request.Department is not null) job.Department = request.Department;
        if (request.EmploymentType is not null) job.EmploymentType = request.EmploymentType.Value;
        if (request.ExperienceMin is not null) job.ExperienceMin = request.ExperienceMin;
        if (request.ExperienceMax is not null) job.ExperienceMax = request.ExperienceMax;
        if (request.SalaryMin is not null) job.SalaryMin = request.SalaryMin;
        if (request.SalaryMax is not null) job.SalaryMax = request.SalaryMax;
        if (request.Location is not null) job.Location = request.Location;
        if (request.WorkMode is not null) job.WorkMode = request.WorkMode.Value;
        if (request.NumberOfOpenings is not null) job.NumberOfOpenings = request.NumberOfOpenings;
        if (request.Description is not null) job.Description = request.Description;
        if (request.Requirements is not null) job.Requirements = request.Requirements;
        if (request.Benefits is not null) job.Benefits = request.Benefits;
        if (request.ApplicationDeadline is not null) job.ApplicationDeadline = request.ApplicationDeadline;

        _unitOfWork.Repository<Job>().Update(job);

        // Replace skills if provided
        if (request.SkillIds is not null)
        {
            var existingSkills = await _unitOfWork.Repository<JobSkill>()
                .FindAsync(js => js.JobId == jobId);

            _unitOfWork.Repository<JobSkill>().DeleteRange(existingSkills);

            var newSkills = request.SkillIds.Select(skillId => new JobSkill
            {
                JobId = jobId,
                SkillId = skillId
            }).ToList();

            await _unitOfWork.Repository<JobSkill>().AddRangeAsync(newSkills);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UpdateAsync completed successfully. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
        return await MapToResponseAsync(job);
    }

    public async Task ChangeStatusAsync(string userId, int jobId, string newStatus, string traceId)
    {
        _logger.LogInformation(
            "Starting ChangeStatusAsync. UserId: {UserId}, JobId: {JobId}, NewStatus: {NewStatus}, TraceId: {TraceId}",
            userId, jobId, newStatus, traceId);

        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(jobId);

        if (job is null)
        {
            _logger.LogWarning("Job not found for status change. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        if (job.PostedByUserId != userId)
        {
            _logger.LogWarning("Unauthorized status change attempt. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);
            throw new ForbiddenException("You are not authorized to change this job's status.");
        }

        if (!Enum.TryParse<JobStatus>(newStatus, true, out var parsedStatus))
        {
            _logger.LogWarning("Invalid status value. NewStatus: {NewStatus}, TraceId: {TraceId}", newStatus, traceId);
            throw new BadRequestException($"Invalid job status: {newStatus}");
        }

        job.Status = parsedStatus;
        _unitOfWork.Repository<Job>().Update(job);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("ChangeStatusAsync completed successfully. JobId: {JobId}, NewStatus: {NewStatus}, TraceId: {TraceId}", jobId, parsedStatus, traceId);
    }

    public async Task DeleteAsync(string userId, int jobId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(jobId);

        if (job is null)
        {
            _logger.LogWarning("Job not found for deletion. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        if (job.PostedByUserId != userId)
        {
            _logger.LogWarning("Unauthorized delete attempt. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);
            throw new ForbiddenException("You are not authorized to delete this job.");
        }

        // Delete associated job skills first
        var jobSkills = await _unitOfWork.Repository<JobSkill>()
            .FindAsync(js => js.JobId == jobId);

        if (jobSkills.Any())
        {
            _unitOfWork.Repository<JobSkill>().DeleteRange(jobSkills);
        }

        _unitOfWork.Repository<Job>().Delete(job);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
    }

    private async Task<JobResponse> MapToResponseAsync(Job job)
    {
        var company = await _unitOfWork.Repository<Company>().GetByIdAsync(job.CompanyId);

        var jobSkills = await _unitOfWork.Repository<JobSkill>()
            .FindAsync(js => js.JobId == job.Id);

        var skillIds = jobSkills.Select(js => js.SkillId).ToList();
        var skills = skillIds.Any()
            ? await _unitOfWork.Repository<Skill>().FindAsync(s => skillIds.Contains(s.Id))
            : new List<Skill>();

        return new JobResponse
        {
            Id = job.Id,
            Title = job.Title,
            CompanyId = job.CompanyId,
            CompanyName = company?.Name ?? "Unknown",
            Department = job.Department,
            EmploymentType = job.EmploymentType,
            ExperienceMin = job.ExperienceMin,
            ExperienceMax = job.ExperienceMax,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            Location = job.Location,
            WorkMode = job.WorkMode,
            NumberOfOpenings = job.NumberOfOpenings,
            Description = job.Description,
            Requirements = job.Requirements,
            Benefits = job.Benefits,
            ApplicationDeadline = job.ApplicationDeadline,
            Status = job.Status,
            IsFeatured = job.IsFeatured,
            Skills = skills.Select(s => new SkillResponse { Id = s.Id, Name = s.Name }).ToList(),
            CreatedAt = job.CreatedAt
        };
    }
}