namespace Currencies.Api.Infrastructure.Exceptions
{
    public class ValidationError
    {
        public ValidationError(string fieldName
            , string errorCode, string errorMessage, params object[] args)
        {
            FieldName = fieldName;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Arguments = args;
        }

        public ValidationError(string errorCode, string errorMessage, params object[] args)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Arguments = args;
        }

        public string FieldName { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public object[] Arguments { get; }
    }
}
