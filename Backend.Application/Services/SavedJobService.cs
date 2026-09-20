using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Jobs;
using Backend.Application.DTOs.SavedJobs;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class SavedJobService : ISavedJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobService _jobService;
    private readonly ILogger<SavedJobService> _logger;

    public SavedJobService(
        IUnitOfWork unitOfWork,
        IJobService jobService,
        ILogger<SavedJobService> logger)
    {
        _unitOfWork = unitOfWork;
        _jobService = jobService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SavedJobResponse>> GetSavedJobsAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetSavedJobsAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var savedJobs = await _unitOfWork.Repository<SavedJob>()
            .FindAsync(sj => sj.UserId == userId);

        var response = new List<SavedJobResponse>();
        foreach (var savedJob in savedJobs)
        {
            var jobResponse = await _jobService.GetByIdAsync(savedJob.JobId, traceId);
            response.Add(new SavedJobResponse
            {
                Id = savedJob.Id,
                JobId = savedJob.JobId,
                Job = jobResponse,
                SavedAt = savedJob.CreatedAt
            });
        }

        _logger.LogInformation("GetSavedJobsAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<SavedJobResponse> SaveJobAsync(string userId, int jobId, string traceId)
    {
        _logger.LogInformation("Starting SaveJobAsync. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

        // Validate job exists
        var job = await _unitOfWork.Repository<Job>().GetByIdAsync(jobId);
        if (job is null)
        {
            _logger.LogWarning("Job not found. JobId: {JobId}, TraceId: {TraceId}", jobId, traceId);
            throw new NotFoundException("Job not found.");
        }

        // Check if already saved
        var existingSavedJob = await _unitOfWork.Repository<SavedJob>()
            .FirstOrDefaultAsync(sj => sj.UserId == userId && sj.JobId == jobId);

        if (existingSavedJob is not null)
        {
            _logger.LogWarning("Job already saved. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);
            throw new BadRequestException("You have already saved this job.");
        }

        var savedJob = new SavedJob
        {
            UserId = userId,
            JobId = jobId
        };

        await _unitOfWork.Repository<SavedJob>().AddAsync(savedJob);
        await _unitOfWork.SaveChangesAsync();

        var jobResponse = await _jobService.GetByIdAsync(jobId, traceId);

        _logger.LogInformation("SaveJobAsync completed successfully. SavedJobId: {SavedJobId}, TraceId: {TraceId}", savedJob.Id, traceId);

        return new SavedJobResponse
        {
            Id = savedJob.Id,
            JobId = savedJob.JobId,
            Job = jobResponse,
            SavedAt = savedJob.CreatedAt
        };
    }

    public async Task UnsaveJobAsync(string userId, int jobId, string traceId)
    {
        _logger.LogInformation("Starting UnsaveJobAsync. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

        var savedJob = await _unitOfWork.Repository<SavedJob>()
            .FirstOrDefaultAsync(sj => sj.UserId == userId && sj.JobId == jobId);

        if (savedJob is null)
        {
            _logger.LogWarning("Saved job not found. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);
            throw new NotFoundException("Saved job not found.");
        }

        _unitOfWork.Repository<SavedJob>().Delete(savedJob);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UnsaveJobAsync completed successfully. TraceId: {TraceId}", traceId);
    }

    public async Task<bool> IsJobSavedAsync(string userId, int jobId, string traceId)
    {
        _logger.LogInformation("Starting IsJobSavedAsync. UserId: {UserId}, JobId: {JobId}, TraceId: {TraceId}", userId, jobId, traceId);

        var exists = await _unitOfWork.Repository<SavedJob>()
            .AnyAsync(sj => sj.UserId == userId && sj.JobId == jobId);

        _logger.LogInformation("IsJobSavedAsync completed successfully. Result: {Result}, TraceId: {TraceId}", exists, traceId);
        return exists;
    }
}