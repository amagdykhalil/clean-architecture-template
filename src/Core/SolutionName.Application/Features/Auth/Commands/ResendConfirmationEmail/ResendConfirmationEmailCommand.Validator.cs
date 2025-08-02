using SolutionName.Application.Common.Validator;

namespace SolutionName.Application.Features.Auth.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
    {
        public ResendConfirmationEmailCommandValidator()
        {
            RuleFor(r => r.Email)
                .SetValidator(new CustomEmailValidator<ResendConfirmationEmailCommand>());
        }
    }
}