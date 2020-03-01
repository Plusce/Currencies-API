
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

        public ApiError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Explanation of the error.
        /// </summary>
        public string Message { get; set; }
    }
}
