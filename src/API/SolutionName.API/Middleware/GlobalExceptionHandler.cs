using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SolutionName.Shared.Keys;
using System.Net;
using System.Text.Json;

namespace SolutionName.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IStringLocalizer<GlobalExceptionHandler> _localizer;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IStringLocalizer<GlobalExceptionHandler> localizer)
        {
            _logger = logger;
            _localizer = localizer;
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

            // Optionally, you could use a localized summary message here
            return ApiResponse.BadRequest(errors);
        }

        private ApiResponse HandleUnauthorizedException()
        {
            return ApiResponse.Unauthorized(new List<ApiErrorResponse>
            {
                new(_localizer[LocalizationKeys.GlobalException.Unauthorized])
            });
        }

        private ApiResponse HandleNotFoundException()
        {
            return ApiResponse.NotFound(new List<ApiErrorResponse>
            {
                new(_localizer[LocalizationKeys.GlobalException.NotFound])
            });
        }

        private ApiResponse HandleBadRequest(Exception ex)
        {
            return ApiResponse.BadRequest(new List<ApiErrorResponse>
            {
                new(_localizer[LocalizationKeys.GlobalException.BadRequest, ex.Message])
            });
        }

        private ApiResponse HandleDbUpdateException(DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed.");

            return ApiResponse.InternalServerError(new List<ApiErrorResponse>
            {
                new(_localizer[LocalizationKeys.GlobalException.DbError])
            });
        }

        private ApiResponse HandleSqlException(SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "SQL Server exception occurred.");

            var (statusCode, message) = sqlEx.Number switch
            {
                2627 or 2601 => (HttpStatusCode.Conflict, _localizer[LocalizationKeys.GlobalException.SqlConflict]), // Unique constraint violation
                547 => (HttpStatusCode.BadRequest, _localizer[LocalizationKeys.GlobalException.SqlFK]), // FK violation
                1205 => (HttpStatusCode.Conflict, _localizer[LocalizationKeys.GlobalException.SqlDeadlock]), // Deadlock
                515 => (HttpStatusCode.BadRequest, _localizer[LocalizationKeys.GlobalException.SqlNotNull]), // Not null violation
                _ => (HttpStatusCode.InternalServerError, _localizer[LocalizationKeys.GlobalException.SqlFallback]) // Fallback
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
                new(_localizer[LocalizationKeys.GlobalException.Internal])
            });
        }
    }
}



