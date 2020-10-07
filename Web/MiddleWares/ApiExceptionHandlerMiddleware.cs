using Web.Exceptions;
using Domain.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Web.MiddleWares
{
    /// <summary>
    /// Middleware for global exception handling
    /// </summary>
    public class ApiExceptionHandlerMiddleware
    {
        readonly RequestDelegate Next;
        readonly IStringLocalizer<Error> ErrorMessageLocalizer;
        readonly ILogger<ApiExceptionHandlerMiddleware> Logger;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="errorMessageLocalizer"></param>
        /// <param name="logger"></param>
        public ApiExceptionHandlerMiddleware(
            RequestDelegate next, 
            IStringLocalizer<Error> errorMessageLocalizer,
            ILogger<ApiExceptionHandlerMiddleware> logger)
        {
            Next = next;           
            ErrorMessageLocalizer = errorMessageLocalizer;
            Logger = logger;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context to invoke.</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                await Next(context);
            }
            catch (Exception ex)
            {
                var problemDetails = GetProblemDetails(ex);
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                context.Response.StatusCode = (int)problemDetails.Status;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, problemDetails.GetType(), jsonSerializerOptions));
            }
        }

        /// <summary>
        /// Maps exception to ProblemDetails
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>ProblemDetails</returns>
        private ProblemDetails GetProblemDetails(Exception exception)
        {
            ProblemDetails problemDetails;
            
            if (exception is DateRangeValidationException dateRangeValidationException)
            {
                var exceptionMessage = ErrorMessageLocalizer[dateRangeValidationException.LocalizableMessage, dateRangeValidationException.FieldName];
                problemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>()
                {
                    { dateRangeValidationException.FieldName, new string[] { exceptionMessage } }
                })
                {
                    Status = dateRangeValidationException.Code,
                    Title = ErrorMessageLocalizer["ValidationError"]
                };
            }
            else if (exception is BaseException baseException)
            {
                problemDetails = new ProblemDetails()
                {
                    Status = baseException.Code,
                    Title = ErrorMessageLocalizer[baseException.LocalizableMessage]
                };
            }
            else
            {
                problemDetails = new ProblemDetails()
                {
                    //Detail = exception.Message,
                    Status = 500,
                    Title = ErrorMessageLocalizer["ServerError"]
                };

                Logger.LogError(exception.Message);
            }

            return problemDetails;
        }
    }
}
