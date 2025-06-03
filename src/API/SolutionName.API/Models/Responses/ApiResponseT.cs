using System.Text.Json.Serialization;

namespace SolutionName.Application.Models.Responses;

/// <summary>
/// Represents a standardized API response that can contain a result of type T.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed class ApiResponse<T> : ApiResponse
{
    [JsonConstructor]
    public ApiResponse(T result, bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
    : base(success, successMessage, statusCode, errors)
    {
        Result = result;
    }

    public ApiResponse()
    {
    }

    public T Result { get; private init; }

    public static ApiResponse<T> Ok(T result) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Result = result };

    public static ApiResponse<T> Ok(T result, string successMessage) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, Result = result, SuccessMessage = successMessage };

    public static ApiResponse<T> Created(T result, string successMessage) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status201Created, Result = result, SuccessMessage = successMessage };

    public override string ToString() =>
        $"IsSuccess: {IsSuccess} | StatusCode: {StatusCode} | Result: {Result} | HasErrors: {Errors.Any()}";

}

