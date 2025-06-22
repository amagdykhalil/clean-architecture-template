using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace SolutionName.Application.Features.Auth.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandHandler : ICommandHandler<ResendConfirmationEmailCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserEmailService _userEmailService;
        private readonly IStringLocalizer<ResendConfirmationEmailCommandHandler> _localizer;
        private readonly string _frontendBaseUrl;

        public ResendConfirmationEmailCommandHandler(
            IIdentityService identityService,
            IUserEmailService userEmailService,
            IStringLocalizer<ResendConfirmationEmailCommandHandler> localizer,
            IConfiguration configuration)
        {
            _identityService = identityService;
            _userEmailService = userEmailService;
            _localizer = localizer;
            _frontendBaseUrl = configuration["Frontend:BaseUrl"] ?? "https://localhost:5173";
        }

        public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Result.Success();
            }

            await SendConfirmationEmailAsync(user, request.Email);
            return Result.Success();
        }

        private async Task SendConfirmationEmailAsync(User user, string email)
        {
            var code = await _identityService.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // Create confirmation link with proper URL structure
            var confirmationLink = $"{_frontendBaseUrl}/api/auth/confirm-email?userId={user.Id}&code={Uri.EscapeDataString(code)}";
            
            await _userEmailService.SendConfirmationLinkAsync(user, email, confirmationLink);
        }
    }
} 