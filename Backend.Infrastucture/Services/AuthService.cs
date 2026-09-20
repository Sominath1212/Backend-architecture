using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Authentication;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse> RegisterAsync(
            RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userManager
                .FindByEmailAsync(email);

            if (existingUser is not null)
            {
                throw new BadRequestException(
                    "A user with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim()
            };

            var result = await _userManager
                .CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(error => error.Description)
                    .ToList();

                throw new BadRequestException(
                    "User registration failed.",
                    errors);
            }

            var roles = await _userManager
                .GetRolesAsync(user);

            var accessToken = _jwtTokenService.GenerateToken(
                user.Id,
                user.Email!,
                user.FirstName ?? string.Empty,
                user.LastName ?? string.Empty,
                roles);

            return new AuthResponse
            {
                AccessToken = accessToken,
                ExpiresAt = _jwtTokenService.GetExpirationTime(),
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty
            };
        }

        public async Task<AuthResponse> LoginAsync(
            LoginRequest request)
        {
            var email = request.Email
                .Trim()
                .ToLowerInvariant();

            var user = await _userManager
                .FindByEmailAsync(email);

            if (user is null)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            var passwordValid = await _userManager
                .CheckPasswordAsync(
                    user,
                    request.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            var roles = await _userManager
                .GetRolesAsync(user);

            var accessToken = _jwtTokenService.GenerateToken(
                user.Id,
                user.Email!,
                user.FirstName ?? string.Empty,
                user.LastName ?? string.Empty,
                roles);

            return new AuthResponse
            {
                AccessToken = accessToken,
                ExpiresAt = _jwtTokenService.GetExpirationTime(),
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty
            };
        }
    }
}