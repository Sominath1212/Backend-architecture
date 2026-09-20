using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(
            string userId,
            string email,
            string firstName,
            string lastName,
            IEnumerable<string> roles);
        DateTime GetExpirationTime();
    }
}
