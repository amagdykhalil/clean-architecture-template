using FluentValidation;
using System.Numerics;

namespace SolutionName.Application.Common.Validators
{
    public static class CustomValidators
    {
        // Extension for numeric types
        public static IRuleBuilderOptions<T, TProperty> GreaterThanZero<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
        where TProperty : INumber<TProperty>
        {
            return ruleBuilder
                .GreaterThan(TProperty.Zero)
                .WithMessage("{PropertyName} must be greater than 0.");
        }

        // Extension for nullable numeric types
        public static IRuleBuilderOptions<T, TProperty?> GreaterThanZero<T, TProperty>(
            this IRuleBuilder<T, TProperty?> ruleBuilder)
            where TProperty : struct, INumber<TProperty>
        {
            return ruleBuilder
                .GreaterThan(TProperty.Zero)
                .WithMessage("{PropertyName} must be greater than 0.")
                .When(x => x != null);
        }
    }
}


