using System.Diagnostics;
using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.CandidateProfile;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Services
{
    public class CandidateProfileService : ICandidateProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CandidateProfileService> _logger;

        public CandidateProfileService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ILogger<CandidateProfileService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<CandidateProfileResponse> GetProfileByUserIdAsync(
            string userId,
            string traceId)
        {
            _logger.LogInformation("Starting GetProfileByUserIdAsync for UserId: {UserId}. TraceId: {TraceId}", userId, traceId);

            var profile = await _unitOfWork.Repository<CandidateProfile>()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile is null)
            {
                _logger.LogWarning("Profile not found for UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw new NotFoundException("Candidate profile not found.");
            }

            var response = MapToResponse(profile);
            _logger.LogInformation("Completed GetProfileByUserIdAsync successfully. TraceId: {TraceId}", traceId);

            return response;
        }

        public async Task<CandidateProfileResponse> CreateProfileAsync(
            string userId,
            CreateCandidateProfileRequest request,
            string traceId)
        {
            _logger.LogInformation("Starting CreateProfileAsync for UserId: {UserId}. TraceId: {TraceId}", userId, traceId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogError("User not found for UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw new NotFoundException("User not found.");
            }

            var existingProfile = await _unitOfWork.Repository<CandidateProfile>()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile is not null)
            {
                _logger.LogWarning("Profile already exists for UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw new BadRequestException("A candidate profile already exists for this user. Use update instead.");
            }

            var profile = new CandidateProfile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Mobile = request.Mobile,
                Location = request.Location,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                CurrentDesignation = request.CurrentDesignation,
                TotalExperience = request.TotalExperience,
                CurrentCompany = request.CurrentCompany,
                CurrentSalary = request.CurrentSalary,
                ExpectedSalary = request.ExpectedSalary,
                NoticePeriod = request.NoticePeriod,
                EmploymentStatus = request.EmploymentStatus,
                PreferredJobType = request.PreferredJobType,
                PreferredLocation = request.PreferredLocation,
                Email = user.Email // Sync email from user
            };

            await _unitOfWork.Repository<CandidateProfile>().AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Completed CreateProfileAsync successfully. ProfileId: {ProfileId}. TraceId: {TraceId}", profile.Id, traceId);

            return MapToResponse(profile);
        }

        public async Task<CandidateProfileResponse> UpdateProfileAsync(
            string userId,
            UpdateCandidateProfileRequest request,
            string traceId)
        {
            _logger.LogInformation("Starting UpdateProfileAsync for UserId: {UserId}. TraceId: {TraceId}", userId, traceId);

            var profile = await _unitOfWork.Repository<CandidateProfile>()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile is null)
            {
                _logger.LogWarning("Profile not found for update. UserId: {UserId}. TraceId: {TraceId}", userId, traceId);
                throw new NotFoundException("Candidate profile not found. Please create a profile first.");
            }

            // Update only provided fields
            if (request.FirstName is not null) profile.FirstName = request.FirstName;
            if (request.LastName is not null) profile.LastName = request.LastName;
            if (request.Mobile is not null) profile.Mobile = request.Mobile;
            if (request.Location is not null) profile.Location = request.Location;
            if (request.DateOfBirth is not null) profile.DateOfBirth = request.DateOfBirth;
            if (request.Gender is not null) profile.Gender = request.Gender;
            if (request.CurrentDesignation is not null) profile.CurrentDesignation = request.CurrentDesignation;
            if (request.TotalExperience is not null) profile.TotalExperience = request.TotalExperience;
            if (request.CurrentCompany is not null) profile.CurrentCompany = request.CurrentCompany;
            if (request.CurrentSalary is not null) profile.CurrentSalary = request.CurrentSalary;
            if (request.ExpectedSalary is not null) profile.ExpectedSalary = request.ExpectedSalary;
            if (request.NoticePeriod is not null) profile.NoticePeriod = request.NoticePeriod;
            if (request.EmploymentStatus is not null) profile.EmploymentStatus = request.EmploymentStatus;
            if (request.PreferredJobType is not null) profile.PreferredJobType = request.PreferredJobType;
            if (request.PreferredLocation is not null) profile.PreferredLocation = request.PreferredLocation;

            _unitOfWork.Repository<CandidateProfile>().Update(profile);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Completed UpdateProfileAsync successfully. ProfileId: {ProfileId}. TraceId: {TraceId}", profile.Id, traceId);

            return MapToResponse(profile);
        }

        private static CandidateProfileResponse MapToResponse(CandidateProfile profile)
        {
            return new CandidateProfileResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email,
                Mobile = profile.Mobile,
                Location = profile.Location,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                CurrentDesignation = profile.CurrentDesignation,
                TotalExperience = profile.TotalExperience,
                CurrentCompany = profile.CurrentCompany,
                CurrentSalary = profile.CurrentSalary,
                ExpectedSalary = profile.ExpectedSalary,
                NoticePeriod = profile.NoticePeriod,
                EmploymentStatus = profile.EmploymentStatus,
                PreferredJobType = profile.PreferredJobType,
                PreferredLocation = profile.PreferredLocation
            };
        }
    }
}