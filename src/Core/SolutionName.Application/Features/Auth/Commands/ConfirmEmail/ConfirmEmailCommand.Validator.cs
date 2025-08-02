using SolutionName.Application.Common.Validator;
using SolutionName.Application.Features.Auth.Common;

namespace SolutionName.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .SetValidator(new IdValidator<ConfirmEmailCommand>());

            RuleFor(x => x.Code)
                .SetValidator(new CodeValidator<ConfirmEmailCommand>());

            RuleFor(x => x.ChangedEmail)
                .SetValidator(new CustomEmailValidator<ConfirmEmailCommand>());
        }
    }
}