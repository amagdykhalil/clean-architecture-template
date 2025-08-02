

using SolutionName.Application.Common.Validator;

namespace SolutionName.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator(
            IIdentityService identityService)
        {
            RuleFor(x => x.UserId)
                .SetValidator(new IdValidator<ChangePasswordCommand>());

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("This field is required.");

            RuleFor(x => x.NewPassword)
                .SetAsyncValidator(new PasswordValidator<ChangePasswordCommand>(identityService));
        }
    }
}
