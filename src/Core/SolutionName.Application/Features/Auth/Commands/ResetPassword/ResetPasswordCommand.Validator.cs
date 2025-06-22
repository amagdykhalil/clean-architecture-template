using SolutionName.Application.Abstractions.UserContext;

namespace SolutionName.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator(
            IStringLocalizer<ResetPasswordCommandValidator> localizer,
            IIdentityService identityService)
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .WithMessage(localizer[LocalizationKeys.Validation.EmailRequired])
                .EmailAddress()
                .WithMessage(localizer[LocalizationKeys.Validation.InvalidEmail]);

            RuleFor(r => r.ResetCode)
                .NotEmpty()
                .WithMessage(localizer[LocalizationKeys.Validation.ResetCodeRequired]);

            RuleFor(r => r.NewPassword)
                .SetAsyncValidator(new PasswordValidator<ResetPasswordCommand>(identityService, localizer));
        }
    }
} 