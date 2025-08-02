using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
namespace SolutionName.Infrastructure.Email
{
    public class UserActionLinkBuilder : IUserActionLinkBuilder
    {
        private readonly IIdentityService _identityService;
        private readonly string _frontendBaseUrl;
        private readonly string _backendBaseUrl;

        public UserActionLinkBuilder(
            IIdentityService identityService,
            IConfiguration configuration)
        {
            _identityService = identityService;
            _frontendBaseUrl = configuration["Frontend:BaseUrl"].TrimEnd('/');
            _backendBaseUrl = configuration["Backend:BaseUrl"].TrimEnd('/');
        }

        public async Task<string> BuildEmailConfirmationLinkAsync(User user, CancellationToken cancellationToken = default)
        {
            var rawCode = await _identityService.GenerateEmailConfirmationTokenAsync(user);
            var bytes = Encoding.UTF8.GetBytes(rawCode);
            var encoded = WebEncoders.Base64UrlEncode(bytes);

            var url = $"{_backendBaseUrl}/api/auth/confirm-email?"
                    + $"userId={user.Id}"
                    + $"&code={Uri.EscapeDataString(encoded)}";
            return url;
        }

        public async Task<string> BuildPasswordResetLinkAsync(User user, string email, CancellationToken cancellationToken = default)
        {
            // generate the code 
            var rawCode = await _identityService.GeneratePasswordResetTokenAsync(user);
            // base64‑url encode
            var bytes = Encoding.UTF8.GetBytes(rawCode);
            var encoded = WebEncoders.Base64UrlEncode(bytes);

            var url = $"{_frontendBaseUrl}/reset-password?"
                    + $"email={Uri.EscapeDataString(email)}"
                    + $"&resetCode={Uri.EscapeDataString(encoded)}";

            return url;
        }

        public async Task<string> BuildChangeEmailConfirmationLinkAsync(User user, string newEmail, CancellationToken cancellationToken = default)
        {
            var rawCode = await _identityService.GenerateChangeEmailTokenAsync(user, newEmail);
            var bytes = Encoding.UTF8.GetBytes(rawCode);
            var encoded = WebEncoders.Base64UrlEncode(bytes);

            var url = $"{_backendBaseUrl}/api/auth/confirm-email?"
                    + $"userId={user.Id}"
                    + $"&code={Uri.EscapeDataString(encoded)}"
                    + $"&changedEmail={Uri.EscapeDataString(newEmail)}";
            return url;
        }
    }

}