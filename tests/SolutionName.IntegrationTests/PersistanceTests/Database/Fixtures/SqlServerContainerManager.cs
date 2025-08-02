using SolutionName.IntegrationTests.PersistanceTests.Database.Configurations;
using DotNet.Testcontainers.Builders;

namespace SolutionName.IntegrationTests.PersistanceTests.Database.Fixtures
{
    public class SqlServerContainerManager : IAsyncLifetime
    {
        public DotNet.Testcontainers.Containers.IContainer DbContainer { get; private set; }
        public DatabaseSettings dbSettings;

        public SqlServerContainerManager(DatabaseSettings dbSettings)
        {
            this.dbSettings = dbSettings;
        }

        public async Task InitializeAsync()
        {
            DbContainer = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SA_PASSWORD", dbSettings.Password)
                .WithPortBinding(1433, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                .Build();

            await DbContainer.StartAsync();

            var mappedPort = DbContainer.GetMappedPublicPort(1433);
            dbSettings.Port = mappedPort;
        }

        public async Task DisposeAsync()
        {
            await DbContainer.DisposeAsync();
        }
    }
}
