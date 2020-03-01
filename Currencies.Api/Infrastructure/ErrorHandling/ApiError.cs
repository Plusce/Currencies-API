
using System.Collections.Generic;

namespace Currencies.Api.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Represents single error.
    /// </summary>
    public class ApiError
    {
        public ApiError()
        {
        }

        public ApiError(string message, IDictionary<string, string> arguments = null)
        {
            Message = message;
            Arguments = arguments;
        }

        /// <summary>
        /// Explanation of the error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional key/value error information
        /// </summary>
        public IDictionary<string, string> Arguments { get; set; }
    }
}
