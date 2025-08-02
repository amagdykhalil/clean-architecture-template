using FluentValidation.Validators;

public class PasswordValidator<T> : AsyncPropertyValidator<T, string>
    where T : class
{
    private const int MaxPasswordLength = 20;
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
        if (!string.IsNullOrEmpty(value) && value.Length > MaxPasswordLength)
        {
            var message = $"Password must not exceed {MaxPasswordLength} characters.";
            context.AddFailure(message);
            return false;
        }

        var validationResult = await _identityService.ValidatePasswordAsync(value);

        if (!validationResult.Succeeded)
        {
            var error = validationResult.Errors.First();
            var message = GetErrorMessage(error.Code);
            context.AddFailure(message);
            return false;
        }

        return true;
    }

    private string GetErrorMessage(string errorCode)
    {
        return errorCode switch
        {
            "PasswordTooShort" => "Password must be at least 8 characters.",
            "PasswordRequiresDigit" => "Password must contain at least one digit.",
            "PasswordRequiresUpper" => "Password must contain at least one uppercase letter.",
            "PasswordRequiresLower" => "Password must contain at least one lowercase letter.",
            "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character.",
            "PasswordTooLong" => $"Password must not exceed {MaxPasswordLength} characters.",
            _ => "The format is invalid."
        };
    }
}
