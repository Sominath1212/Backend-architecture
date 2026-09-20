using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Common.Exceptions
{
    public class UnauthorizedException : ApplicationException { public UnauthorizedException(string message = "Unauthorized access.") : base(message, 401) { } }
}
