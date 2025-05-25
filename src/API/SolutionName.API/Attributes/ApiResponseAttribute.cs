using Microsoft.AspNetCore.Mvc;
using SolutionName.Application.Models.Responses;

namespace SolutionName.API.Attributes;

/// <summary>
/// Custom attribute that simplifies API response documentation by automatically wrapping response types in ApiResponse<T>.
/// This attribute extends ProducesResponseTypeAttribute to provide a more concise way to document API responses.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// [ApiResponse(StatusCodes.Status200OK, typeof(UserDto))] // Documents ApiResponse<UserDto>
/// [ApiResponse(StatusCodes.Status400BadRequest)] // Documents ApiResponse<object>
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ApiResponseAttribute : ProducesResponseTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the ApiResponseAttribute class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code for the response.</param>
    /// <param name="type">The type of data in the response. If null, defaults to object.</param>
    public ApiResponseAttribute(int statusCode, Type? type = null) 
        : base(type == null ? typeof(ApiResponse<object>) : typeof(ApiResponse<>).MakeGenericType(type), statusCode)
    {
    }
} 