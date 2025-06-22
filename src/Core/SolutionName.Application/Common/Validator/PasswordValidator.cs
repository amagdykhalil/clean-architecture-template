using FluentValidation.Validators;

public class PasswordValidator<T> : AsyncPropertyValidator<T, string>
    where T : class
{
    private const int MaxPasswordLength = 20;
    private readonly IIdentityService _identityService;
    private readonly IStringLocalizer _localizer;

    public PasswordValidator(IIdentityService identityService, IStringLocalizer localizer)
    {
        _identityService = identityService;
        _localizer = localizer;
    }

    public override string Name => "PasswordValidator";

    public override async Task<bool> IsValidAsync(
        ValidationContext<T> context,
        string value,
        CancellationToken cancellation)
    {
        // Check max length first
        if (!string.IsNullOrEmpty(value) && value.Length > MaxPasswordLength)
        {
            var message = _localizer[LocalizationKeys.Validation.PasswordTooLong, MaxPasswordLength];
            context.AddFailure(message);
            return false;
        }

        var validationResult = await _identityService.ValidatePasswordAsync(value);

        if (!validationResult.Succeeded)
        {
            var error = validationResult.Errors.First();
            var message = GetLocalizationMessage(error.Code, _localizer);
            context.AddFailure(message);
            return false;
        }

        return true;
    }

    private string GetLocalizationMessage(string errorCode, IStringLocalizer localizer)
    {
        return errorCode switch
        {
            "PasswordTooShort" => localizer[LocalizationKeys.Validation.PasswordTooShort, 8],
            "PasswordRequiresDigit" => localizer[LocalizationKeys.Validation.PasswordRequiresDigit],
            "PasswordRequiresUpper" => localizer[LocalizationKeys.Validation.PasswordRequiresUpper],
            "PasswordRequiresLower" => localizer[LocalizationKeys.Validation.PasswordRequiresLower],
            "PasswordRequiresNonAlphanumeric" => localizer[LocalizationKeys.Validation.PasswordRequiresNonAlphanumeric],
            "PasswordTooLong" => localizer[LocalizationKeys.Validation.PasswordTooLong, MaxPasswordLength],
            _ => errorCode // fallback
        };
    }
}
