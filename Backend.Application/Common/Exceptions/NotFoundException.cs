using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Common.Exceptions
{
    public class NotFoundException : ApplicationException { public NotFoundException(string message) : base(message, 404) { } }
}
