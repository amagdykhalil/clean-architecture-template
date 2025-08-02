using FluentValidation.Validators;

namespace SolutionName.Application.Common.Validator
{
    public class IdNullableValidator<T> : PropertyValidator<T, int?>
    {
        public override string Name => "IdNullableValidator";

        public override bool IsValid(ValidationContext<T> context, int? value)
        {
            if (value == null)
                return true; // Not our concern if empty

            if (value <= 1)
            {
                context.AddFailure("This field must be greater than or equal 1.");
                return false;
            }

            return true;
        }
    }
}

