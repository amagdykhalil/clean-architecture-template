using SolutionName.IntegrationTests.PersistanceTests.Database.Configurations;
using Microsoft.Extensions.Configuration;

namespace SolutionName.IntegrationTests.PersistanceTests.Database.Fixtures
{
    public class DatabaseConfigProvider
    {
        public IConfiguration Configuration { get; }

        public DatabaseConfigProvider()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("PersistanceTests/Database/Configurations/databasetestsettings.json", optional: false)
                .Build();
        }

        public DatabaseSettings GetDatabaseSettings()
            => Configuration.GetSection("Database").Get<DatabaseSettings>()
               ?? throw new InvalidOperationException("Database settings not found.");
    }
}
