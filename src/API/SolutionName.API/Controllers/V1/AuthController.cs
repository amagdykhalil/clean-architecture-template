using SolutionName.Application.Features.Auth;
using SolutionName.Application.Features.Auth.Commands.ConfirmEmail;
using SolutionName.Application.Features.Auth.Commands.ForgotPassword;
using SolutionName.Application.Features.Auth.Commands.Login;
using SolutionName.Application.Features.Auth.Commands.RefreshToken;
using SolutionName.Application.Features.Auth.Commands.ResendConfirmationEmail;
using SolutionName.Application.Features.Auth.Commands.ResetPassword;
using SolutionName.Application.Features.Auth.Commands.RevokeToken;

namespace SolutionName.API.Controllers.V1
{
    /// <summary>
    /// Controller for handling authentication operations including login, token refresh, token revocation,
    /// email confirmation, and password reset.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private const string RefreshTokenCookieName = "RefreshToken";

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user and returns access token along with refresh token.
        /// </summary>
        /// <param name="command">The login credentials containing email and password.</param>
        /// <returns>
        /// Returns AuthDTO containing access token and user information on success.
        /// Sets refresh token in an HTTP-only cookie for subsequent token refresh operations.
        /// </returns>
        [HttpPost("login")]
        [ApiResponse(StatusCodes.Status200OK, typeof(AuthDTO))]
        [ApiResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetRefreshTokenCookie(response.Value.RefreshToken, response.Value.RefreshTokenExpiration);
            }

            return response.ToActionResult();
        }

        /// <summary>
        /// Refreshes the access token using the refresh token stored in the cookie.
        /// </summary>
        /// <returns>
        /// Returns new AuthDTO with fresh access token and refresh token.
        /// Updates the refresh token cookie with the new token.
        /// </returns>
        [HttpPost("refresh-token")]
        [ApiResponse(StatusCodes.Status200OK, typeof(AuthDTO))]
        [ApiResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Request.Cookies[RefreshTokenCookieName];
            var command = new RefreshTokenCommand(refreshToken);
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetRefreshTokenCookie(response.Value.RefreshToken, response.Value.RefreshTokenExpiration);
            }

            return response.ToActionResult();
        }

        /// <summary>
        /// Revokes the current refresh token and removes it from the cookie.
        /// </summary>
        /// <returns>Returns 204 No Content on successful token revocation.</returns>
        [HttpPost("revoke-token")]
        [ApiResponse(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var command = new RevokeTokenCommand(refreshToken);
            var response = await _mediator.Send(command);

            HttpContext.Response.Cookies.Delete(RefreshTokenCookieName);
            return response.ToActionResult();
        }

        /// <summary>
        /// Confirms a user's email address using the provided confirmation code.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="code">The confirmation code.</param>
        /// <param name="changedEmail">Optional new email address if changing email.</param>
        /// <returns>Returns 200 OK on successful email confirmation.</returns>
        [HttpGet("confirm-email")]
        [ApiResponse(StatusCodes.Status200OK)]
        [ApiResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string code,
            [FromQuery] string? changedEmail = null)
        {
            var command = new ConfirmEmailCommand(userId, code, changedEmail);
            var response = await _mediator.Send(command);
            return response.ToActionResult();
        }

        /// <summary>
        /// Resends email confirmation to the specified email address.
        /// </summary>
        /// <param name="command">The resend confirmation email request.</param>
        /// <returns>Returns 200 OK on successful email sending.</returns>
        [HttpPost("resend-confirmation-email")]
        [ApiResponse(StatusCodes.Status200OK)]
        [ApiResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailCommand command)
        {
            var response = await _mediator.Send(command);
            return response.ToActionResult();
        }

        /// <summary>
        /// Sends a password reset link to the specified email address.
        /// </summary>
        /// <param name="command">The forgot password request.</param>
        /// <returns>Returns 200 OK on successful email sending.</returns>
        [HttpPost("forgot-password")]
        [ApiResponse(StatusCodes.Status200OK)]
        [ApiResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var response = await _mediator.Send(command);
            return response.ToActionResult();
        }

        /// <summary>
        /// Resets a user's password using the provided reset code.
        /// </summary>
        /// <param name="command">The reset password request.</param>
        /// <returns>Returns 200 OK on successful password reset.</returns>
        [HttpPost("reset-password")]
        [ApiResponse(StatusCodes.Status200OK)]
        [ApiResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var response = await _mediator.Send(command);
            return response.ToActionResult();
        }

        /// <summary>
        /// Sets the refresh token in an HTTP-only cookie with secure options.
        /// </summary>
        /// <param name="token">The refresh token to store.</param>
        /// <param name="expiresOn">The expiration date of the token.</param>
        private void SetRefreshTokenCookie(string token, DateTime expiresOn)
        {

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access to the cookie
                Secure = true,   // Only sent over HTTPS
                IsEssential = true,
                SameSite = SameSiteMode.None, // Allows cross-site requests
                Expires = expiresOn
            };

            HttpContext.Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
        }
    }
}

