using Backend.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(
       RegisterRequest request);

        Task<AuthResponse> LoginAsync(
            LoginRequest request);
    }

}
