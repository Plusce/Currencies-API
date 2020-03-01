using System;
using System.Collections.Generic;
using System.Linq;

namespace Currencies.Api.Infrastructure.Exceptions
{
    public class ValidationException : Exception
    {
        public List<ValidationError> ValidationErrors { get; } = new List<ValidationError>();

        public ValidationException()
        {

        }

        public ValidationException(string fieldName, string errorCode, string errorMessage, params object[] args)
            : base($"Validation failed with error code {errorCode}: {errorMessage}")
        {
            ValidationErrors.Add(new ValidationError(fieldName, errorCode, errorMessage, args));
        }

        public ValidationException(string errorCode, string errorMessage, params object[] args)
            : base($"Validation failed with error code {errorCode}: {errorMessage}")
        {
            ValidationErrors.Add(new ValidationError(errorCode, errorMessage, args));
        }

        public ValidationException(params ValidationError[] errors) : base($"Validation failed with {string.Join(",", errors.Select(x => x.ToString()))}")
        {
            foreach (var error in errors)
            {
                ValidationErrors.Add(error);
            }
        }
    }
}
