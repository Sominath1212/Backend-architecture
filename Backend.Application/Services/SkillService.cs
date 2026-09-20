using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Skills;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class SkillService : ISkillService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SkillService> _logger;

    public SkillService(IUnitOfWork unitOfWork, ILogger<SkillService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CandidateSkillResponse>> GetCandidateSkillsAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetCandidateSkillsAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var candidateSkills = await _unitOfWork.Repository<CandidateSkill>()
            .FindAsync(cs => cs.UserId == userId);

        // Fetch master skill names to avoid N+1 query issues
        var skillIds = candidateSkills.Select(cs => cs.SkillId).Distinct().ToList();
        var skills = await _unitOfWork.Repository<Skill>().FindAsync(s => skillIds.Contains(s.Id));
        var skillDict = skills.ToDictionary(s => s.Id, s => s.Name);

        var response = candidateSkills.Select(cs => new CandidateSkillResponse
        {
            Id = cs.Id,
            UserId = cs.UserId,
            SkillId = cs.SkillId,
            SkillName = skillDict.TryGetValue(cs.SkillId, out var name) ? name : "Unknown",
            ProficiencyLevel = cs.ProficiencyLevel,
            YearsOfExperience = cs.YearsOfExperience,
            LastUsed = cs.LastUsed
        }).ToList();

        _logger.LogInformation("GetCandidateSkillsAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }

    public async Task<CandidateSkillResponse> CreateCandidateSkillAsync(string userId, CreateCandidateSkillRequest request, string traceId)
    {
        _logger.LogInformation("Starting CreateCandidateSkillAsync. UserId: {UserId}, SkillId: {SkillId}, TraceId: {TraceId}", userId, request.SkillId, traceId);

        var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(request.SkillId);
        if (skill is null)
        {
            _logger.LogWarning("Master skill not found. SkillId: {SkillId}, TraceId: {TraceId}", request.SkillId, traceId);
            throw new NotFoundException("Skill not found.");
        }

        var existing = await _unitOfWork.Repository<CandidateSkill>()
            .FirstOrDefaultAsync(cs => cs.UserId == userId && cs.SkillId == request.SkillId);

        if (existing is not null)
        {
            _logger.LogWarning("Candidate already has this skill. UserId: {UserId}, SkillId: {SkillId}, TraceId: {TraceId}", userId, request.SkillId, traceId);
            throw new BadRequestException("You have already added this skill. Please update it instead.");
        }

        var candidateSkill = new CandidateSkill
        {
            UserId = userId,
            SkillId = request.SkillId,
            ProficiencyLevel = request.ProficiencyLevel,
            YearsOfExperience = request.YearsOfExperience,
            LastUsed = request.LastUsed
        };

        await _unitOfWork.Repository<CandidateSkill>().AddAsync(candidateSkill);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateCandidateSkillAsync completed successfully. CandidateSkillId: {Id}, TraceId: {TraceId}", candidateSkill.Id, traceId);

        return new CandidateSkillResponse
        {
            Id = candidateSkill.Id,
            UserId = candidateSkill.UserId,
            SkillId = candidateSkill.SkillId,
            SkillName = skill.Name,
            ProficiencyLevel = candidateSkill.ProficiencyLevel,
            YearsOfExperience = candidateSkill.YearsOfExperience,
            LastUsed = candidateSkill.LastUsed
        };
    }

    public async Task<CandidateSkillResponse> UpdateCandidateSkillAsync(string userId, int candidateSkillId, UpdateCandidateSkillRequest request, string traceId)
    {
        _logger.LogInformation("Starting UpdateCandidateSkillAsync. UserId: {UserId}, CandidateSkillId: {Id}, TraceId: {TraceId}", userId, candidateSkillId, traceId);

        var candidateSkill = await _unitOfWork.Repository<CandidateSkill>()
            .FirstOrDefaultAsync(cs => cs.Id == candidateSkillId && cs.UserId == userId);

        if (candidateSkill is null)
        {
            _logger.LogWarning("Candidate skill not found. UserId: {UserId}, CandidateSkillId: {Id}, TraceId: {TraceId}", userId, candidateSkillId, traceId);
            throw new NotFoundException("Candidate skill not found.");
        }

        if (request.ProficiencyLevel is not null) candidateSkill.ProficiencyLevel = request.ProficiencyLevel.Value;
        if (request.YearsOfExperience is not null) candidateSkill.YearsOfExperience = request.YearsOfExperience;
        if (request.LastUsed is not null) candidateSkill.LastUsed = request.LastUsed;

        _unitOfWork.Repository<CandidateSkill>().Update(candidateSkill);
        await _unitOfWork.SaveChangesAsync();

        var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(candidateSkill.SkillId);

        _logger.LogInformation("UpdateCandidateSkillAsync completed successfully. CandidateSkillId: {Id}, TraceId: {TraceId}", candidateSkill.Id, traceId);

        return new CandidateSkillResponse
        {
            Id = candidateSkill.Id,
            UserId = candidateSkill.UserId,
            SkillId = candidateSkill.SkillId,
            SkillName = skill?.Name ?? "Unknown",
            ProficiencyLevel = candidateSkill.ProficiencyLevel,
            YearsOfExperience = candidateSkill.YearsOfExperience,
            LastUsed = candidateSkill.LastUsed
        };
    }

    public async Task DeleteCandidateSkillAsync(string userId, int candidateSkillId, string traceId)
    {
        _logger.LogInformation("Starting DeleteCandidateSkillAsync. UserId: {UserId}, CandidateSkillId: {Id}, TraceId: {TraceId}", userId, candidateSkillId, traceId);

        var candidateSkill = await _unitOfWork.Repository<CandidateSkill>()
            .FirstOrDefaultAsync(cs => cs.Id == candidateSkillId && cs.UserId == userId);

        if (candidateSkill is null)
        {
            _logger.LogWarning("Candidate skill not found for deletion. UserId: {UserId}, CandidateSkillId: {Id}, TraceId: {TraceId}", userId, candidateSkillId, traceId);
            throw new NotFoundException("Candidate skill not found.");
        }

        _unitOfWork.Repository<CandidateSkill>().Delete(candidateSkill);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteCandidateSkillAsync completed successfully. CandidateSkillId: {Id}, TraceId: {TraceId}", candidateSkillId, traceId);
    }

    public async Task<IReadOnlyList<SkillResponse>> GetAllMasterSkillsAsync(string traceId)
    {
        _logger.LogInformation("Starting GetAllMasterSkillsAsync. TraceId: {TraceId}", traceId);

        var skills = await _unitOfWork.Repository<Skill>()
            .FindAsync(s => s.IsActive);

        var response = skills.Select(s => new SkillResponse { Id = s.Id, Name = s.Name }).ToList();

        _logger.LogInformation("GetAllMasterSkillsAsync completed successfully. Count: {Count}, TraceId: {TraceId}", response.Count, traceId);
        return response;
    }
}