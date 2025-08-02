using FluentValidation.Validators;

namespace SolutionName.Application.Common.Validator
{
    public class IdValidator<T> : PropertyValidator<T, int>
    {
        public override string Name => "IdValidator";

        public override bool IsValid(ValidationContext<T> context, int value)
        {

            if (value < 1)
            {
                context.AddFailure($"This field must be greater than or equal 1.");
                return false;
            }

            return true;
        }
    }
}