using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Currencies.Api.Infrastructure.ErrorHandling
{
    public class ApiErrorResult
    {
        /// <summary>
        /// List of errors
        /// </summary>
        public List<ApiError> Errors { get; set; } = new List<ApiError>();

        public ApiErrorResult()
        {
        }

        public ApiErrorResult(string errorMessage)
        {
            Errors.Add(new ApiError(errorMessage));
        }

        public ApiErrorResult(params ApiError[] errors)
        {
            Errors.AddRange(errors);
        }

        /// <summary>
        /// Allows to create ApiErrorResult object with ApiError array which are retrieved from ValidationException.
        /// </summary>
        /// <param name="ex">Contains error list with ValidationError type.</param>
        /// <returns></returns>
        public static ApiErrorResult CreateFrom(ValidationException ex)
        {
            var errors = ex.ValidationErrors
                ?.Select(MapValidationErrorToApiError)
                .ToArray();

            ApiErrorResult result = new ApiErrorResult(errors);
            return result;

            ApiError MapValidationErrorToApiError(ValidationError validationError)
            {
                var arguments = !string.IsNullOrWhiteSpace(validationError.FieldName)
                    ? new Dictionary<string, string> { { "FieldName: ", validationError.FieldName } }
                    : null;

                return new ApiError(validationError.ErrorMessage, arguments);
            }
        }

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
