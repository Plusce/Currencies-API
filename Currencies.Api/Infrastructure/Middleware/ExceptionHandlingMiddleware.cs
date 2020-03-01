using Currencies.Api.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Currencies.Api.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ValidationException validationException:
                        await RespondWithValidationError(context, validationException, HttpStatusCode.BadRequest);
                        return;
                    default:
                        await RespondWithError(context, "Internal Server Error", HttpStatusCode.InternalServerError);
                        break;
                }
            }
        }

        private async Task RespondWithError(
          HttpContext context,
          string errorMessage,
          HttpStatusCode statusCode)
        {
            var apiResult = new ApiErrorResult(errorMessage);
            await RespondWithError(context, apiResult, statusCode);
        }

        private async Task RespondWithValidationError(HttpContext context, ValidationException validationException, HttpStatusCode statusCode)
        {
            var apiErrorResult = ApiErrorResult.CreateFrom(validationException);
            await RespondWithError(context, apiErrorResult, statusCode);
        }

        private static async Task RespondWithError(
            HttpContext context,
            ApiErrorResult apiResult,
            HttpStatusCode statusCode)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = @"application/json";
            string apiResultString = apiResult.ToJsonString();
            await context.Response.WriteAsync(apiResultString);
        }
    }
}
