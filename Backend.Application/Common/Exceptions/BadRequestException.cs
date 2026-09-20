using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Common.Exceptions
{
    public class BadRequestException : ApplicationException { public BadRequestException(string message, object? errors = null) : base(message, 400, errors) { } }
}
