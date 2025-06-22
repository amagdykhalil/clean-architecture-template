namespace SolutionName.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator(
            IStringLocalizer<ForgotPasswordCommandValidator> localizer)
        {
            RuleFor(f => f.Email)
                .NotEmpty()
                .WithMessage(localizer[LocalizationKeys.Validation.EmailRequired])
                .EmailAddress()
                .WithMessage(localizer[LocalizationKeys.Validation.InvalidEmail]);
        }
    }
} 