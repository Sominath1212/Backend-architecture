using Backend.Application.DTOs.CandidateProfile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Interfaces
{
    public interface ICandidateProfileService
    {
        Task<CandidateProfileResponse> GetProfileByUserIdAsync(string userId, string traceId);
        Task<CandidateProfileResponse> CreateProfileAsync(string userId, CreateCandidateProfileRequest request, string traceId);
        Task<CandidateProfileResponse> UpdateProfileAsync(string userId, UpdateCandidateProfileRequest request, string traceId);
    }
}
