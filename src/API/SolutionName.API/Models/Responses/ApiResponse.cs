using System.Text.Json.Serialization;

namespace SolutionName.Application.Models.Responses;

/// <summary>
/// Represents a standardized API response that can contain a result of type T.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public class ApiResponse
{
    [JsonConstructor]
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

    public bool IsSuccess { get; protected init; }
    public string SuccessMessage { get; protected init; }
    public int StatusCode { get; protected init; }
    public IEnumerable<ApiErrorResponse> Errors { get; private init; } = [];

    public static ApiResponse Ok() =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK };

    public static ApiResponse Ok(string successMessage) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    public static ApiResponse Created() =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status201Created };

    public static ApiResponse Created(string successMessage) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status201Created, SuccessMessage = successMessage };

    public static ApiResponse BadRequest() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status400BadRequest };

    public static ApiResponse BadRequest(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateErrors(errorMessage) };

    public static ApiResponse BadRequest(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    public static ApiResponse NoContent() => new() { IsSuccess = true, StatusCode = StatusCodes.Status204NoContent };

    public static ApiResponse Unauthorized() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status401Unauthorized };

    public static ApiResponse Unauthorized(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Unauthorized(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    public static ApiResponse Forbidden() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status403Forbidden };

    public static ApiResponse Forbidden(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Forbidden(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status403Forbidden, Errors = errors };

    public static ApiResponse NotFound() =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status404NotFound };

    public static ApiResponse NotFound(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateErrors(errorMessage) };

    public static ApiResponse NotFound(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    public static ApiResponse InternalServerError(string errorMessage) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateErrors(errorMessage) };

    public static ApiResponse InternalServerError(IEnumerable<ApiErrorResponse> errors) =>
        new() { IsSuccess = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = errors };

    private static ApiErrorResponse[] CreateErrors(string errorMessage) =>
        [new ApiErrorResponse(errorMessage)];

    public override string ToString() =>
        $"IsSuccess: {IsSuccess} | StatusCode: {StatusCode} | HasErrors: {Errors.Any()}";

}

