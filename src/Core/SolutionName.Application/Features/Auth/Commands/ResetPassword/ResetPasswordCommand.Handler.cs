using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SolutionName.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(
        IIdentityService identityService,
        IStringLocalizer<ResetPasswordCommandHandler> localizer)
        : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await identityService.FindByEmailAsync(request.Email);

            if (user is null || !(await identityService.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Result.Error(localizer[LocalizationKeys.Auth.InvalidResetCode]);
            }

            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
                result = await identityService.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                return Result.Error(localizer[LocalizationKeys.Auth.InvalidResetCode]);
            }

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Error(errorMessage);
            }

            return Result.Success();
        }
    }
}