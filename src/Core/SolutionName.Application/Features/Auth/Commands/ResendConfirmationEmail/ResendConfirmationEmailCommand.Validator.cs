namespace SolutionName.Application.Features.Auth.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
    {
        public ResendConfirmationEmailCommandValidator(
            IStringLocalizer<ResendConfirmationEmailCommandValidator> localizer)
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .WithMessage(localizer[LocalizationKeys.Validation.EmailRequired])
                .EmailAddress()
                .WithMessage(localizer[LocalizationKeys.Validation.InvalidEmail]);
        }
    }
} 