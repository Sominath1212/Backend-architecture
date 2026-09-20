using Backend.Application.Common.Models;
using Backend.Application.DTOs.Authentication;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(
                ApiResponse<AuthResponse>.SuccessResponse(
                    result,
                    "User registered successfully.",
                    StatusCodes.Status200OK));
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(
                ApiResponse<AuthResponse>.SuccessResponse(
                    result,
                    "Login successful.",
                    StatusCodes.Status200OK));
        }
    }
}