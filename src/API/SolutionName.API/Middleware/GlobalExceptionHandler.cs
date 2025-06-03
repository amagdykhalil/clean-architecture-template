using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace SolutionName.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = exception switch
            {
                ValidationException validationException => HandleValidationException(validationException),
                UnauthorizedAccessException => HandleUnauthorizedException(),
                KeyNotFoundException => HandleNotFoundException(),
                ArgumentNullException argNull => HandleBadRequest(argNull),
                ArgumentException argEx => HandleBadRequest(argEx),
                InvalidOperationException invalidOp => HandleBadRequest(invalidOp),
                NotSupportedException notSupported => HandleBadRequest(notSupported),
                ApplicationException appEx => HandleBadRequest(appEx),

                DbUpdateException dbUpdateEx => HandleDbUpdateException(dbUpdateEx),
                SqlException sqlEx => HandleSqlException(sqlEx),

                _ => HandleInternalError(exception)
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = response.StatusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await httpContext.Response.WriteAsync(json, cancellationToken);
            return true;
        }

        private static ApiResponse HandleValidationException(ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => new ApiErrorResponse(e.ErrorMessage))
                .ToList();

            return ApiResponse.BadRequest(errors);
        }

        private static ApiResponse HandleUnauthorizedException()
        {
            return ApiResponse.Unauthorized(new List<ApiErrorResponse>
            {
                new("You are not authorized to access this resource.")
            });
        }

        private static ApiResponse HandleNotFoundException()
        {
            return ApiResponse.NotFound(new List<ApiErrorResponse>
            {
                new("The requested resource was not found.")
            });
        }

        private static ApiResponse HandleBadRequest(Exception ex)
        {
            return ApiResponse.BadRequest(new List<ApiErrorResponse>
            {
                new(ex.Message)
            });
        }

        private ApiResponse HandleDbUpdateException(DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed.");

            return ApiResponse.InternalServerError(new List<ApiErrorResponse>
            {
                new("A database error occurred. Please try again later.")
            });
        }

        private ApiResponse HandleSqlException(SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "SQL Server exception occurred.");

            var (statusCode, message) = sqlEx.Number switch
            {
                2627 or 2601 => (HttpStatusCode.Conflict, "Resource already exists."), // Unique constraint violation
                547 => (HttpStatusCode.BadRequest, "Cannot delete resource because it’s referenced elsewhere."), // FK violation
                1205 => (HttpStatusCode.Conflict, "Request failed due to a database conflict. Please retry."), // Deadlock
                515 => (HttpStatusCode.BadRequest, "Missing required data: a field was left empty."), // Not null violation
                _ => (HttpStatusCode.InternalServerError, "A database error occurred. Please contact support.") // Fallback
            };

            return new ApiResponse(false, "", (int)statusCode, new List<ApiErrorResponse>
            {
                new(message)
            });
        }


        private ApiResponse HandleInternalError(Exception ex)
        {
            _logger.LogError(ex, "Unhandled internal exception.");

            return ApiResponse.InternalServerError(new List<ApiErrorResponse>
            {
                new("An internal server error occurred. Please try again later.")
            });
        }
    }
}


