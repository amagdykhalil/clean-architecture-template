using FluentValidation.Validators;

namespace SolutionName.Application.Common.Validator
{
    public class PhoneNumberValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => "PhoneNumberValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                context.AddFailure("Password is required");
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^(010|011|012|015)[0-9]{8}$"))
            {
                context.AddFailure("The format is invalid.");
                return false;
            }
            return true;
        }
    }
}