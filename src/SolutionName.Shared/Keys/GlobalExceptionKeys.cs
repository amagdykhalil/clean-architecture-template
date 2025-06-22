namespace SolutionName.Shared.Keys
{
    public static partial class LocalizationKeys
    {
        public static class GlobalException
        {
            public static string ValidationError { get; private set; }
            public static string Unauthorized { get; private set; }
            public static string NotFound { get; private set; }
            public static string BadRequest { get; private set; }
            public static string DbError { get; private set; }
            public static string SqlConflict { get; private set; }
            public static string SqlFK { get; private set; }
            public static string SqlDeadlock { get; private set; }
            public static string SqlNotNull { get; private set; }
            public static string SqlFallback { get; private set; }
            public static string Internal { get; private set; }
        }
    }
}