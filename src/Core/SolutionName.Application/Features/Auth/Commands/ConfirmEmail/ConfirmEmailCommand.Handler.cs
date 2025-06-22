using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SolutionName.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler(
        IIdentityService identityService,
        IStringLocalizer<ConfirmEmailCommandHandler> localizer)
        : ICommandHandler<ConfirmEmailCommand>
    {
        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await identityService.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return Result.Error(localizer[LocalizationKeys.Auth.InvalidToken]);
            }

            string code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch (FormatException)
            {
                return Result.Error(localizer[LocalizationKeys.Auth.InvalidToken]);
            }

            IdentityResult result;

            if (string.IsNullOrEmpty(request.ChangedEmail))
            {
                result = await identityService.ConfirmEmailAsync(user, code);
            }
            else
            {
                // As with Identity UI, email and user name are one and the same. So when we update the email,
                // we need to update the user name.
                result = await identityService.ChangeEmailAsync(user, request.ChangedEmail, code);

                if (result.Succeeded)
                {
                    result = await identityService.SetUserNameAsync(user, request.ChangedEmail);
                }
            }

            if (!result.Succeeded)
            {
                return Result.Error(localizer[LocalizationKeys.Auth.InvalidToken]);
            }

            return Result.Success();
        }
    }
} 