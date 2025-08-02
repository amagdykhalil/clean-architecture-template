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

        private ApiResponse HandleValidationException(ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => new ApiErrorResponse(e.ErrorMessage))
                .ToList();

            return ApiResponse.BadRequest(errors);
        }

        private ApiResponse HandleUnauthorizedException()
        {
            return ApiResponse.Unauthorized(new List<ApiErrorResponse>
            {
                new("Unauthorized access.")
            });
        }

        private ApiResponse HandleNotFoundException()
        {
            return ApiResponse.NotFound(new List<ApiErrorResponse>
            {
                new("Resource not found.")
            });
        }

        private ApiResponse HandleBadRequest(Exception ex)
        {
            return ApiResponse.BadRequest(new List<ApiErrorResponse>
            {
                new($"Bad request")
            });
        }

        private ApiResponse HandleDbUpdateException(DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed.");

            return ApiResponse.InternalServerError(new List<ApiErrorResponse>
            {
                new("A database error occurred during update.")
            });
        }

        private ApiResponse HandleSqlException(SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "SQL Server exception occurred.");

            var (statusCode, message) = sqlEx.Number switch
            {
                2627 or 2601 => (HttpStatusCode.Conflict, "A record with the same key already exists."),
                547 => (HttpStatusCode.BadRequest, "This operation violates a foreign key constraint."),
                1205 => (HttpStatusCode.Conflict, "A database deadlock occurred. Please try again."),
                515 => (HttpStatusCode.BadRequest, "A required field is missing a value."),
                _ => (HttpStatusCode.InternalServerError, "A database error occurred.")
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
                new("An unexpected error occurred. Please try again later.")
            });
        }
    }
}
