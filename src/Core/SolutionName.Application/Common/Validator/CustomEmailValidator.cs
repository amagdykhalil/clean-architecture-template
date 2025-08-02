using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace SolutionName.Application.Common.Validator
{
    public class CustomEmailValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => "EmailValidator";

        private readonly bool _requered;
        private const int _maxLength = 256;
        private static readonly Regex _emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);



        public CustomEmailValidator(bool requered = false)
        {
            _requered = requered;
        }

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (_requered && string.IsNullOrEmpty(value))
            {
                context.AddFailure("Email is required");
                return false;
            }


            if (value.Length > _maxLength)
            {
                context.AddFailure($"This field must be less than or equal {_maxLength}.");
                return false;
            }

            if (!_emailRegex.IsMatch(value))
            {
                // Ensure LocalizationKeys.Validation.InvalidEmailFormat exists in your localization keys
                context.AddFailure("The format is invalid.");
                return false;
            }

            return true;
        }
    }
}

