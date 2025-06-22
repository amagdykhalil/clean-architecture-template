namespace SolutionName.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator(IStringLocalizer<LoginCommandValidator> localizer)
        {
            RuleFor(l => l.Email)
                .NotEmpty()
                .WithMessage(localizer[LocalizationKeys.Validation.EmailRequired])
                .EmailAddress()
                .WithMessage(localizer[LocalizationKeys.Validation.InvalidEmail]);

            RuleFor(l => l.Password)
                .NotEmpty()
                .WithMessage(localizer[LocalizationKeys.Validation.PasswordRequired]);
        }
    }
}
