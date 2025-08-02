using SolutionName.Application.Common.Validator;

namespace SolutionName.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator(IIdentityService identityService)
        {
            RuleFor(l => l.Email)
                .SetValidator(new CustomEmailValidator<LoginCommand>(true));

            RuleFor(l => l.Password)
                .NotEmpty()
                .WithMessage("This field is required.")
                .MaximumLength(50)
                .WithMessage("Password must not exceed 50 characters.");
        }
    }
}
