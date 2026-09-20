using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Resumes;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Constants;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class ResumeService : IResumeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<ResumeService> _logger;

    public ResumeService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        ILogger<ResumeService> logger)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ResumeResponse>> GetAllByUserIdAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetAllByUserIdAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var resumes = await _unitOfWork.Repository<Resume>()
            .FindAsync(r => r.UserId == userId);

        var response = resumes.Select(MapToResponse).ToList();

        _logger.LogInformation("GetAllByUserIdAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<ResumeResponse> UploadResumeAsync(string userId, Stream fileStream, string fileName, string contentType, string traceId)
    {
        _logger.LogInformation("Starting UploadResumeAsync. UserId: {UserId}, FileName: {FileName}, TraceId: {TraceId}", userId, fileName, traceId);

        // Validate file extension
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!ResumeConstants.AllowedFileTypes.Contains(fileExtension))
        {
            _logger.LogWarning("Invalid file type. FileName: {FileName}, TraceId: {TraceId}", fileName, traceId);
            throw new BadRequestException($"Invalid file type. Allowed types: {string.Join(", ", ResumeConstants.AllowedFileTypes)}");
        }

        // Upload file to storage
        var storagePath = await _fileStorageService.UploadFileAsync(fileStream, fileName, contentType);

        var resume = new Resume
        {
            UserId = userId,
            OriginalFileName = fileName,
            FileSize = fileStream.Length,
            FileType = contentType,
            StoragePath = storagePath,
            IsPrimary = false
        };

        // Check if this is the first resume - make it primary
        var existingResumes = await _unitOfWork.Repository<Resume>()
            .FindAsync(r => r.UserId == userId);

        if (!existingResumes.Any())
        {
            resume.IsPrimary = true;
        }

        await _unitOfWork.Repository<Resume>().AddAsync(resume);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("UploadResumeAsync completed successfully. ResumeId: {ResumeId}, TraceId: {TraceId}", resume.Id, traceId);
        return MapToResponse(resume);
    }

    public async Task<ResumeResponse> SetPrimaryAsync(string userId, int resumeId, string traceId)
    {
        _logger.LogInformation("Starting SetPrimaryAsync. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);

        var resume = await _unitOfWork.Repository<Resume>()
            .FirstOrDefaultAsync(r => r.Id == resumeId && r.UserId == userId);

        if (resume is null)
        {
            _logger.LogWarning("Resume not found. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);
            throw new NotFoundException("Resume not found.");
        }

        // Remove primary flag from all other resumes
        var allResumes = await _unitOfWork.Repository<Resume>()
            .FindAsync(r => r.UserId == userId);

        foreach (var r in allResumes)
        {
            if (r.IsPrimary)
            {
                r.IsPrimary = false;
                _unitOfWork.Repository<Resume>().Update(r);
            }
        }

        // Set this resume as primary
        resume.IsPrimary = true;
        _unitOfWork.Repository<Resume>().Update(resume);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("SetPrimaryAsync completed successfully. ResumeId: {ResumeId}, TraceId: {TraceId}", resumeId, traceId);
        return MapToResponse(resume);
    }

    public async Task<(Stream FileStream, string FileName, string ContentType)> DownloadResumeAsync(string userId, int resumeId, string traceId)
    {
        _logger.LogInformation("Starting DownloadResumeAsync. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);

        var resume = await _unitOfWork.Repository<Resume>()
            .FirstOrDefaultAsync(r => r.Id == resumeId && r.UserId == userId);

        if (resume is null)
        {
            _logger.LogWarning("Resume not found for download. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);
            throw new NotFoundException("Resume not found.");
        }

        var fileStream = await _fileStorageService.DownloadFileAsync(resume.StoragePath);
        if (fileStream is null)
        {
            _logger.LogError("File not found in storage. StoragePath: {StoragePath}, TraceId: {TraceId}", resume.StoragePath, traceId);
            throw new NotFoundException("Resume file not found in storage.");
        }

        _logger.LogInformation("DownloadResumeAsync completed successfully. ResumeId: {ResumeId}, TraceId: {TraceId}", resumeId, traceId);
        return (fileStream, resume.OriginalFileName, resume.FileType);
    }

    public async Task DeleteResumeAsync(string userId, int resumeId, string traceId)
    {
        _logger.LogInformation("Starting DeleteResumeAsync. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);

        var resume = await _unitOfWork.Repository<Resume>()
            .FirstOrDefaultAsync(r => r.Id == resumeId && r.UserId == userId);

        if (resume is null)
        {
            _logger.LogWarning("Resume not found for deletion. UserId: {UserId}, ResumeId: {ResumeId}, TraceId: {TraceId}", userId, resumeId, traceId);
            throw new NotFoundException("Resume not found.");
        }

        // Delete file from storage
        await _fileStorageService.DeleteFileAsync(resume.StoragePath);

        // Delete resume record
        _unitOfWork.Repository<Resume>().Delete(resume);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteResumeAsync completed successfully. ResumeId: {ResumeId}, TraceId: {TraceId}", resumeId, traceId);
    }

    private static ResumeResponse MapToResponse(Resume resume)
    {
        return new ResumeResponse
        {
            Id = resume.Id,
            UserId = resume.UserId,
            OriginalFileName = resume.OriginalFileName,
            FileSize = resume.FileSize,
            FileType = resume.FileType,
            IsPrimary = resume.IsPrimary,
            CreatedAt = resume.CreatedAt
        };
    }
}