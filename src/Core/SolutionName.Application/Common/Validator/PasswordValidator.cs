using SolutionName.Application.Abstractions.UserContext;
using FluentValidation;
using FluentValidation.Validators;

namespace SolutionName.Application.Common.Validators
{
    /// <summary>
    /// Custom validator for password fields that integrates with the identity service.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    public class PasswordValidator<T>
         : AsyncPropertyValidator<T, string>
           where T : class
    {
        private readonly IIdentityService _identityService;

        public PasswordValidator(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public override string Name => "PasswordValidator";

        public override async Task<bool> IsValidAsync(
            ValidationContext<T> context,
            string value,
            CancellationToken cancellation)
        {
            var validationResult = await _identityService
                                         .ValidatePasswordAsync(value);

            if (!validationResult.Succeeded)
            {
                // report the first failure message
                context.AddFailure(
                    validationResult.Errors
                                    .First()
                                    .Description
                );
                return false;
            }

            return true;
        }
    }
}


