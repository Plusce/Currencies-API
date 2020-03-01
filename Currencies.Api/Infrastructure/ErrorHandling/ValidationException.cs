using System;
using System.Collections.Generic;
using System.Linq;

namespace Currencies.Api.Infrastructure.ErrorHandling
{
    public class ValidationException : Exception
    {
        public List<ValidationError> ValidationErrors { get; } = new List<ValidationError>();

        public ValidationException()
        {

        }

        public ValidationException(IEnumerable<ValidationError> validationErrors)
        {
            foreach (var error in validationErrors)
            {
                ValidationErrors.Add(error);
            }
        }
    }
}
