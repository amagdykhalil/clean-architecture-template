namespace ARC.Application.Common.Validators
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, TProperty> GreaterThanZero<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            IStringLocalizer localizer)
            where TProperty : System.Numerics.INumber<TProperty>
        {
            return ruleBuilder
                .GreaterThan(TProperty.Zero)
                .WithMessage(x => localizer[LocalizationKeys.Validation.MustBeGreaterThanZero, "{PropertyName}"]);
        }

        public static IRuleBuilderOptions<T, TProperty?> GreaterThanZero<T, TProperty>(
            this IRuleBuilder<T, TProperty?> ruleBuilder,
            IStringLocalizer localizer)
            where TProperty : struct, System.Numerics.INumber<TProperty>
        {
            return ruleBuilder
                .GreaterThan(TProperty.Zero)
                .WithMessage(x => localizer[LocalizationKeys.Validation.MustBeGreaterThanZero, "{PropertyName}"])
                .When(x => x != null);
        }
    }
}
