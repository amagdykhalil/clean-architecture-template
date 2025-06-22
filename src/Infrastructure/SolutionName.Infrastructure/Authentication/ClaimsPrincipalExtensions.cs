using System.Security.Claims;

namespace Infrastructure.Authentication;

/// <summary>
/// Extension methods for working with ClaimsPrincipal objects.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the claims principal.
    /// </summary>
    /// <param name="principal">The claims principal to extract the user ID from.</param>
    /// <returns>The user ID as an integer.</returns>
    /// <exception cref="ApplicationException">Thrown when the user ID is not available in the claims.</exception>
    public static int GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userId, out int parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("User id is unavailable");
    }
}
