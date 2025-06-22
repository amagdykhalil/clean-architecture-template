

namespace SolutionName.Application.Abstractions.Infrastructure
{
    /// <summary>
    /// Interface for managing JWT token generation and validation.
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Creates a JWT access token for the specified user.
        /// </summary>
        /// <param name="user">The user to create the token for.</param>
        /// <returns>A JWT token string.</returns>
        Task<string> Create(User user);

        /// <summary>
        /// Gets the expiration time for access tokens.
        /// </summary>
        /// <returns>The DateTime when the token will expire.</returns>
        DateTime GetAccessTokenExpiration();
    }
}


