using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace SolutionName.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserEmailService _userEmailService;
        private readonly IStringLocalizer<ForgotPasswordCommandHandler> _localizer;
        private readonly string _frontendBaseUrl;

        public ForgotPasswordCommandHandler(
            IIdentityService identityService,
            IUserEmailService userEmailService,
            IStringLocalizer<ForgotPasswordCommandHandler> localizer,
            IConfiguration configuration)
        {
            _identityService = identityService;
            _userEmailService = userEmailService;
            _localizer = localizer;
            _frontendBaseUrl = configuration["Frontend:BaseUrl"] ?? "https://localhost:5173";
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email);

            if (user is not null && await _identityService.IsEmailConfirmedAsync(user))
            {
                var code = await _identityService.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var link = $"{_frontendBaseUrl}/reset-password?email={Uri.EscapeDataString(request.Email)}&resetCode={Uri.EscapeDataString(code)}";
                await _userEmailService.SendPasswordResetLinkAsync(user, request.Email, link);
            }

            // Don't reveal that the user does not exist or is not confirmed
            return Result.Success();
        }
    }
}