using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Applications;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IUnitOfWork unitOfWork, ILogger<ApplicationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResponse> ApplyAsync(string candidateUserId, CreateApplicationRequest request, string traceId)
    {
        _logger.LogInformation(
            "Starting ApplyAsync. CandidateUserId: {CandidateUserId}, JobId: {JobId}, TraceId: {TraceId}",
            candidateUserId, request.JobId, traceId);

        // Validate job exists and is published
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(request.JobId);
        if (job is null)
        {
            _logger.LogWarning("Job not found. JobId: {JobId}, TraceId: {TraceId}", request.JobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        if (job.Status != JobStatus.Published)
        {
            _logger.LogWarning("Job is not accepting applications. JobId: {JobId}, Status: {Status}, TraceId: {TraceId}", request.JobId, job.Status, traceId);
            throw new BadRequestException("This job is not currently accepting applications.");
        }

        if (job.ApplicationDeadline.HasValue && job.ApplicationDeadline.Value < DateTime.UtcNow)
        {
            _logger.LogWarning("Application deadline has passed. JobId: {JobId}, Deadline: {Deadline}, TraceId: {TraceId}", request.JobId, job.ApplicationDeadline, traceId);
            throw new BadRequestException("The application deadline for this job has passed.");
        }

        // Validate resume exists
        var resume = await _unitOfWork.Repository<Resume>().GetByIdAsync(request.ResumeId);
        if (resume is null || resume.UserId != candidateUserId)
        {
            _logger.LogWarning("Resume not found or access denied. ResumeId: {ResumeId}, TraceId: {TraceId}", request.ResumeId, traceId);
            throw new NotFoundException("Resume not found.");
        }

        // Check for duplicate application
        var existingApplication = await _unitOfWork.Repository<JobApplication>()
            .FirstOrDefaultAsync(a => a.JobId == request.JobId && a.CandidateUserId == candidateUserId);

        if (existingApplication is not null)
        {
            _logger.LogWarning("Duplicate application attempt. CandidateUserId: {CandidateUserId}, JobId: {JobId}, TraceId: {TraceId}", candidateUserId, request.JobId, traceId);
            throw new BadRequestException("You have already applied to this job.");
        }

        // Create application
        var application = new JobApplication
        {
            JobId = request.JobId,
            CandidateUserId = candidateUserId,
            ResumeId = request.ResumeId,
            CoverLetter = request.CoverLetter,
            ExpectedSalary = request.ExpectedSalary,
            NoticePeriod = request.NoticePeriod,
            Status = ApplicationStatus.Applied
        };

        await _unitOfWork.Repository<JobApplication>().AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        // Create initial status history
        var statusHistory = new ApplicationStatusHistory
        {
            ApplicationId = application.Id,
            Status = ApplicationStatus.Applied,
            ChangedByUserId = candidateUserId,
            ChangedAt = DateTime.UtcNow,
            Notes = "Application submitted"
        };

        await _unitOfWork.Repository<ApplicationStatusHistory>().AddAsync(statusHistory);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("ApplyAsync completed successfully. ApplicationId: {ApplicationId}, TraceId: {TraceId}", application.Id, traceId);
        return await MapToResponseAsync(application);
    }

    public async Task<IReadOnlyList<ApplicationResponse>> GetMyApplicationsAsync(string candidateUserId, string traceId)
    {
        _logger.LogInformation("Starting GetMyApplicationsAsync. CandidateUserId: {CandidateUserId}, TraceId: {TraceId}", candidateUserId, traceId);

        var applications = await _unitOfWork.Repository<JobApplication>()
            .FindAsync(a => a.CandidateUserId == candidateUserId);

        var response = new List<ApplicationResponse>();
        foreach (var app in applications)
        {
            response.Add(await MapToResponseAsync(app));
        }

        _logger.LogInformation("GetMyApplicationsAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<ApplicationResponse> GetMyApplicationByIdAsync(string candidateUserId, int applicationId, string traceId)
    {
        _logger.LogInformation("Starting GetMyApplicationByIdAsync. CandidateUserId: {CandidateUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", candidateUserId, applicationId, traceId);

        var application = await _unitOfWork.Repository<JobApplication>()
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.CandidateUserId == candidateUserId);

        if (application is null)
        {
            _logger.LogWarning("Application not found or access denied. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
            throw new NotFoundException("Application not found.");
        }

        _logger.LogInformation("GetMyApplicationByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return await MapToResponseAsync(application);
    }

    public async Task WithdrawAsync(string candidateUserId, int applicationId, string traceId)
    {
        _logger.LogInformation("Starting WithdrawAsync. CandidateUserId: {CandidateUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", candidateUserId, applicationId, traceId);

        var application = await _unitOfWork.Repository<JobApplication>()
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.CandidateUserId == candidateUserId);

        if (application is null)
        {
            _logger.LogWarning("Application not found or access denied. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
            throw new NotFoundException("Application not found.");
        }

        if (application.Status == ApplicationStatus.Withdrawn)
        {
            _logger.LogWarning("Application already withdrawn. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
            throw new BadRequestException("Application is already withdrawn.");
        }

        application.Status = ApplicationStatus.Withdrawn;
        _unitOfWork.Repository<JobApplication>().Update(application);

        // Add status history
        var statusHistory = new ApplicationStatusHistory
        {
            ApplicationId = application.Id,
            Status = ApplicationStatus.Withdrawn,
            ChangedByUserId = candidateUserId,
            ChangedAt = DateTime.UtcNow,
            Notes = "Application withdrawn by candidate"
        };

        await _unitOfWork.Repository<ApplicationStatusHistory>().AddAsync(statusHistory);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("WithdrawAsync completed successfully. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
    }

    public async Task<PagedResult<ApplicationResponse>> GetApplicationsByJobIdAsync(string recruiterUserId, int jobId, string traceId)
    {
        _logger.LogInformation("Starting GetApplicationsByJobIdAsync. RecruiterUserId: {RecruiterUserId}, JobId: {JobId}, TraceId: {TraceId}", recruiterUserId, jobId, traceId);

        // Verify recruiter owns this job
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(jobId);
        if (job is null)
        {
            _logger.LogWarning("Job not found. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        if (job.PostedByUserId != recruiterUserId)
        {
            _logger.LogWarning("Unauthorized access attempt. RecruiterUserId: {RecruiterUserId}, JobId: {JobId}, TraceId: {TraceId}", recruiterUserId, jobId, traceId);
            throw new ForbiddenException("You are not authorized to view applications for this job.");
        }

        var applications = await _unitOfWork.Repository<JobApplication>()
            .FindAsync(a => a.JobId == jobId);

        var response = new List<ApplicationResponse>();
        foreach (var app in applications)
        {
            response.Add(await MapToResponseAsync(app));
        }

        _logger.LogInformation("GetApplicationsByJobIdAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);

        return new PagedResult<ApplicationResponse>
        {
            Items = response,
            PageNumber = 1,
            PageSize = response.Count,
            TotalCount = response.Count
        };
    }

    public async Task<ApplicationResponse> GetApplicationByIdAsync(string recruiterUserId, int applicationId, string traceId)
    {
        _logger.LogInformation("Starting GetApplicationByIdAsync. RecruiterUserId: {RecruiterUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", recruiterUserId, applicationId, traceId);

        var application = await _unitOfWork.Repository<JobApplication>()
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application is null)
        {
            _logger.LogWarning("Application not found. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
            throw new NotFoundException("Application not found.");
        }

        // Verify recruiter owns the job
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(application.JobId);
        if (job is null || job.PostedByUserId != recruiterUserId)
        {
            _logger.LogWarning("Unauthorized access attempt. RecruiterUserId: {RecruiterUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", recruiterUserId, applicationId, traceId);
            throw new ForbiddenException("You are not authorized to view this application.");
        }

        _logger.LogInformation("GetApplicationByIdAsync completed successfully. TraceId: {TraceId}", traceId);
        return await MapToResponseAsync(application);
    }

    public async Task<ApplicationResponse> UpdateStatusAsync(string recruiterUserId, int applicationId, UpdateApplicationStatusRequest request, string traceId)
    {
        _logger.LogInformation(
            "Starting UpdateStatusAsync. RecruiterUserId: {RecruiterUserId}, ApplicationId: {ApplicationId}, NewStatus: {NewStatus}, TraceId: {TraceId}",
            recruiterUserId, applicationId, request.Status, traceId);

        var application = await _unitOfWork.Repository<JobApplication>()
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application is null)
        {
            _logger.LogWarning("Application not found. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
            throw new NotFoundException("Application not found.");
        }

        // Verify recruiter owns the job
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(application.JobId);
        if (job is null || job.PostedByUserId != recruiterUserId)
        {
            _logger.LogWarning("Unauthorized status update attempt. RecruiterUserId: {RecruiterUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", recruiterUserId, applicationId, traceId);
            throw new ForbiddenException("You are not authorized to update this application.");
        }

        // Update status
        application.Status = request.Status;
        if (request.RecruiterNotes is not null)
        {
            application.RecruiterNotes = request.RecruiterNotes;
        }

        _unitOfWork.Repository<JobApplication>().Update(application);

        // Add status history
        var statusHistory = new ApplicationStatusHistory
        {
            ApplicationId = application.Id,
            Status = request.Status,
            ChangedByUserId = recruiterUserId,
            ChangedAt = DateTime.UtcNow,
            Notes = request.StatusChangeNotes
        };

        await _unitOfWork.Repository<ApplicationStatusHistory>().AddAsync(statusHistory);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UpdateStatusAsync completed successfully. ApplicationId: {ApplicationId}, NewStatus: {NewStatus}, TraceId: {TraceId}", applicationId, request.Status, traceId);
        return await MapToResponseAsync(application);
    }

    public async Task<IReadOnlyList<ApplicationStatusHistoryResponse>> GetStatusHistoryAsync(string recruiterUserId, int applicationId, string traceId)
    {
        _logger.LogInformation("Starting GetStatusHistoryAsync. RecruiterUserId: {RecruiterUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", recruiterUserId, applicationId, traceId);

        var application = await _unitOfWork.Repository<JobApplication>()
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application is null)
        {
            _logger.LogWarning("Application not found. ApplicationId: {ApplicationId}, TraceId: {TraceId}", applicationId, traceId);
            throw new NotFoundException("Application not found.");
        }

        // Verify recruiter owns the job
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(application.JobId);
        if (job is null || job.PostedByUserId != recruiterUserId)
        {
            _logger.LogWarning("Unauthorized access attempt. RecruiterUserId: {RecruiterUserId}, ApplicationId: {ApplicationId}, TraceId: {TraceId}", recruiterUserId, applicationId, traceId);
            throw new ForbiddenException("You are not authorized to view this application's history.");
        }

        var history = await _unitOfWork.Repository<ApplicationStatusHistory>()
            .FindAsync(h => h.ApplicationId == applicationId);

        var response = history
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new ApplicationStatusHistoryResponse
            {
                Id = h.Id,
                Status = h.Status,
                ChangedByUserId = h.ChangedByUserId,
                ChangedAt = h.ChangedAt,
                Notes = h.Notes
            })
            .ToList();

        _logger.LogInformation("GetStatusHistoryAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    private async Task<ApplicationResponse> MapToResponseAsync(JobApplication application)
    {
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(application.JobId);
        var company = job is not null ? await _unitOfWork.Repository<Company>().GetByIdAsync(job.CompanyId) : null;

        var candidate = await _unitOfWork.Repository<CandidateProfile>()
            .FirstOrDefaultAsync(cp => cp.UserId == application.CandidateUserId);

        var lastStatusChange = await _unitOfWork.Repository<ApplicationStatusHistory>()
            .FindAsync(h => h.ApplicationId == application.Id);

        var latestChange = lastStatusChange.OrderByDescending(h => h.ChangedAt).FirstOrDefault();

        return new ApplicationResponse
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = job?.Title ?? "Unknown",
            CompanyId = job?.CompanyId ?? 0,
            CompanyName = company?.Name ?? "Unknown",
            CandidateUserId = application.CandidateUserId,
            CandidateName = candidate is not null ? $"{candidate.FirstName} {candidate.LastName}" : "Unknown",
            CandidateEmail = candidate?.Email ?? "Unknown",
            ResumeId = application.ResumeId ?? 0,
            CoverLetter = application.CoverLetter,
            ExpectedSalary = application.ExpectedSalary,
            NoticePeriod = application.NoticePeriod,
            Status = application.Status,
            RecruiterNotes = application.RecruiterNotes,
            AppliedAt = application.CreatedAt,
            LastStatusChangedAt = latestChange?.ChangedAt
        };
    }
}