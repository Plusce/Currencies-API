namespace Currencies.Api.Infrastructure.ErrorHandling
{
    public class ValidationError
    {
        public ValidationError(string fieldName, string errorMessage)
        {
            FieldName = fieldName;
            ErrorMessage = errorMessage;
        }

        public string FieldName { get; }

        public string ErrorMessage { get; }
    }
}
