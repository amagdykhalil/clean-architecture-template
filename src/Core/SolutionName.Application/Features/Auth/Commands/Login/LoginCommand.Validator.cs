namespace SolutionName.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(l => l.Email).EmailAddress();
            RuleFor(l => l.PasswordHash).NotEmpty();
        }
    }
}
