using SolutionName.Application.Abstractions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SolutionName.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(
        IIdentityService identityService,
        IUserEmailService userEmailService,
        IUserActionLinkBuilder userActionLinkBuilder)
        : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await identityService.FindByEmailIncludePersonAsync(request.Email, cancellationToken);

            if (user is null || !(await identityService.IsEmailConfirmedAsync(user, cancellationToken)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Result.Success();
            }

            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
                result = await identityService.ResetPasswordAsync(user, code, request.NewPassword, cancellationToken);
            }
            catch (FormatException)
            {
                return Result.Success();
            }

            if (!result.Succeeded)
            {
                return Result.Success();
            }

            var resetLink = await userActionLinkBuilder.BuildPasswordResetLinkAsync(user, user.Email, cancellationToken);
            await userEmailService.SendPasswordChangedNotificationAsync(user, user.Email, resetLink);

            return Result.Success();
        }
    }
}