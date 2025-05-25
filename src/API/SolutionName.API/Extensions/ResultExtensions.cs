using SolutionName.Application.Models.Responses;

namespace SolutionName.API.Extensions;

internal static class ResultExtensions
{
    /// <summary>
    /// Converts an ApiResponse<object> object to an IActionResult.
    /// </summary>
    /// <param name="response">The ApiResponse object to convert.</param>
    /// <returns>An IActionResult representing the ApiResponse object.</returns>
    public static IActionResult ToActionResult(this ApiResponse<object> response)
    {
        return response.StatusCode switch
        {
            StatusCodes.Status200OK => new OkObjectResult(response),
            StatusCodes.Status201Created => new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status201Created
            },
            _ => response.ToHttpNonSuccessResult()
        };
    }


    /// <summary>
    /// Converts an ApiResponse{T} to an IActionResult.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="response">The response to convert.</param>
    /// <returns>An IActionResult representing the response.</returns>
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
    {
        return response.StatusCode switch
        {
            StatusCodes.Status200OK => new OkObjectResult(response),
            StatusCodes.Status201Created => new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status201Created
            },
            _ => response.ToHttpNonSuccessResult()
        };
    }


    private static IActionResult ToHttpNonSuccessResult<T>(this ApiResponse<T> response)
    {
        return response.StatusCode switch
        {
            StatusCodes.Status400BadRequest => new BadRequestObjectResult(response),
            StatusCodes.Status401Unauthorized => new UnauthorizedObjectResult(response),
            StatusCodes.Status403Forbidden => new ForbidResult(),
            StatusCodes.Status404NotFound => new NotFoundObjectResult(response),
            StatusCodes.Status204NoContent => new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent },
            StatusCodes.Status500InternalServerError => new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError },
            _ => new BadRequestObjectResult(response)
        };
    }
}

