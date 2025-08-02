using SolutionName.Application.Common.Validator;

namespace SolutionName.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(f => f.Email)
                .SetValidator(new CustomEmailValidator<ForgotPasswordCommand>( true));
        }
    }
}