using System.Reflection;

namespace SolutionName.Persistence.Extensions
{
    public static class SqlMigrationHelper
    {
        private const string EnvVar1 = "ASPNETCORE_ENVIRONMENT";
        private const string EnvVar2 = "DOTNET_ENVIRONMENT";

        /// <summary>
        /// Returns true if either ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT
        /// is set to "Testing" (case‑insensitive).
        /// </summary>
        public static bool IsTesting()
        {
            var env = Environment.GetEnvironmentVariable(EnvVar1)
                      ?? Environment.GetEnvironmentVariable(EnvVar2);

            return env?.Equals("Testing", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Loads the text of a .sql file from a “scripts” folder that’s been copied
        /// alongside the assembly into the output directory.
        /// </summary>
        public static string LoadSql(string fileName)
        {

            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                          ?? throw new InvalidOperationException("Can't find assembly location");

            var sqlPath = Path.Combine(baseDir, "scripts", fileName);
            if (!File.Exists(sqlPath))
                throw new FileNotFoundException($"SQL script not found: {sqlPath}", sqlPath);

            return File.ReadAllText(sqlPath);
        }
    }


}
