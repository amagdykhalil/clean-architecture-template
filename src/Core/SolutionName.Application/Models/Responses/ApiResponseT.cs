using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace SolutionName.Application.Models.Responses;

/// <summary>
/// Represents a standardized API response that can contain a result of type T.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed class ApiResponse<T>
{
    [JsonConstructor]
    public ApiResponse(T result, bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)

    {
        Result = result;
        IsSuccess = success;
        SuccessMessage = successMessage;
        StatusCode = statusCode;
        Errors = errors;
    }

    public ApiResponse(bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
    {
        IsSuccess = success;
        SuccessMessage = successMessage;
        StatusCode = statusCode;
        Errors = errors;
    }

    public ApiResponse()
    {
    }

    /// <summary>
    /// Gets the result data. Will be ignore in the JSON response if null.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T Result { get; private init; }
    public bool IsSuccess { get; protected init; }
    public string SuccessMessage { get; protected init; }
    public int StatusCode { get; protected init; }
    public IEnumerable<ApiErrorResponse> Errors { get; private init; } = [];

    public static ApiResponse<T> Ok(T result) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Result = result };

    public static ApiResponse<T> Ok(T result, string successMessage) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Result = result, SuccessMessage = successMessage };
        
    public static ApiResponse<T> Created(T result) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status201Created, Result = result };

    public static ApiResponse<T> Ok() =>
       new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK };

    public static ApiResponse<T> Ok(string successMessage) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    public static ApiResponse<T> Created() =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status201Created };

    public static ApiResponse<T> BadRequest() => 
        new() { IsSuccess = false, StatusCode = StatusCodes.Status400BadRequest };

    public static ApiResponse<T> BadRequest(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateErrors(errorMessage) };

    public static ApiResponse<T> BadRequest(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    public static ApiResponse<T> NoContent() => new() { IsSuccess = true, StatusCode = StatusCodes.Status204NoContent };

    public static ApiResponse<T> Unauthorized() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status401Unauthorized };

    public static ApiResponse<T> Unauthorized(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateErrors(errorMessage) };

    public static ApiResponse<T> Unauthorized(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    public static ApiResponse<T> Forbidden() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status403Forbidden };

    public static ApiResponse<T> Forbidden(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateErrors(errorMessage) };

    public static ApiResponse<T> Forbidden(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status403Forbidden, Errors = errors };

    public static ApiResponse<T> NotFound() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status404NotFound };

    public static ApiResponse<T> NotFound(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateErrors(errorMessage) };

    public static ApiResponse<T> NotFound(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    public static ApiResponse<T> InternalServerError(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateErrors(errorMessage) };

    public static ApiResponse<T> InternalServerError(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = errors };

    private static ApiErrorResponse[] CreateErrors(string errorMessage) =>
        [new ApiErrorResponse(errorMessage)];

    public override string ToString() =>
        $"IsSuccess: {IsSuccess} | StatusCode: {StatusCode} | Result: {Result} | HasErrors: {Errors.Any()}";

}

