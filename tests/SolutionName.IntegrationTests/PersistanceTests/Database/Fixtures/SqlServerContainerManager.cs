using SolutionName.IntegrationTests.PersistanceTests.Database.Configurations;
using DotNet.Testcontainers.Builders;

namespace SolutionName.IntegrationTests.PersistanceTests.Database.Fixtures
{
    public class SqlServerContainerManager : IAsyncLifetime
    {
        public DotNet.Testcontainers.Containers.IContainer DbContainer { get; private set; }
        private readonly DatabaseSettings _dbSettings;

        public SqlServerContainerManager(DatabaseSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public async Task InitializeAsync()
        {
            DbContainer = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SA_PASSWORD", _dbSettings.Password)
                .WithPortBinding(_dbSettings.Port, 1433)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                .Build();

            await DbContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await DbContainer.DisposeAsync();
        }
    }

}
