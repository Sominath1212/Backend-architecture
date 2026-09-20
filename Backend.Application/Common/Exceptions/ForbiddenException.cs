using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Common.Exceptions
{
    public class ForbiddenException : ApplicationException { public ForbiddenException(string message = "You do not have permission to perform this action.") : base(message, 403) { } }
}
