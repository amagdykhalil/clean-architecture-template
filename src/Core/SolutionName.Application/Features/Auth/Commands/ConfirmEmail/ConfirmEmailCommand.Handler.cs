using SolutionName.Application.Abstractions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SolutionName.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler(
        IIdentityService identityService,
        IUserEmailService userEmailService,
        IUserActionLinkBuilder userActionLinkBuilder)
        : ICommandHandler<ConfirmEmailCommand, string>
    {
        public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await identityService.GetUserByIdIncludePersonAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                return Result<string>.Success(null);
            }

            string code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch (FormatException)
            {
                return Result<string>.Success(null);
            }

            IdentityResult result;

            if (string.IsNullOrEmpty(request.ChangedEmail))
            {

                result = await identityService.ConfirmEmailAsync(user, code, cancellationToken);
            }
            else
            {
                // As with Identity UI, email and user name are one and the same. So when we update the email,
                // we need to update the user name.
                var oldEmail = user.Email;
                result = await identityService.ChangeEmailAsync(user, request.ChangedEmail, code, cancellationToken);

                if (result.Succeeded)
                {
                    result = await identityService.SetUserNameAsync(user, request.ChangedEmail, cancellationToken);
                    await userEmailService.SendEmailChangedNotificationAsync(user, oldEmail, request.ChangedEmail);
                }
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                var url = await userActionLinkBuilder.BuildPasswordResetLinkAsync(user, user.Email, cancellationToken);
                await userEmailService.SendCreatePasswordLinkAsync(user, user.Email, url);
                return Result<string>.Success(url);
            }

            if (!result.Succeeded)
            {
                return Result<string>.Success(null);
            }

            return Result<string>.Success(null);
        }
    }
}