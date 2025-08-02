using SolutionName.Application.Common.Validator;
using FluentValidation;

namespace SolutionName.Application.Features.Auth.Commands.ChangeEmail
{
    public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
    {
        public ChangeEmailCommandValidator()
        {
            RuleFor(x => x.NewEmail)
                .SetValidator(new CustomEmailValidator<ChangeEmailCommand>( true));
        }
    }
} 