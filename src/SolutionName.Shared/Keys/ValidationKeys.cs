namespace SolutionName.Shared.Keys
{
    public static partial class LocalizationKeys
    {

        public static class Validation
        {
            public static string EmailRequired { get; private set; }
            public static string InvalidEmail { get; private set; }
            public static string PasswordRequired { get; private set; }
            public static string MustBeGreaterThanZero { get; private set; }
            public static string PasswordTooShort { get; private set; }
            public static string PasswordRequiresDigit { get; private set; }
            public static string PasswordRequiresUpper { get; private set; }
            public static string PasswordRequiresLower { get; private set; }
            public static string PasswordRequiresNonAlphanumeric { get; private set; }
            public static string PasswordTooLong { get; private set; }
            public static string ResetCodeRequired { get; private set; }
        }
    }
}
