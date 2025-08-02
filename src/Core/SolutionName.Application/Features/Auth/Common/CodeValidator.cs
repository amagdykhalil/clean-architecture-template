using FluentValidation.Validators;

namespace SolutionName.Application.Features.Auth.Common
{
    public class CodeValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => "CodeValidator";

        private const int MaxLength = 400;

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                context.AddFailure("This field is required.");
                return false;
            }

            if (value.Length > MaxLength)
            {
                context.AddFailure($"This field must be less than or equal {MaxLength}.");
                return false;
            }

            return true;
        }
    }
}
