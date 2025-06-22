using SolutionName.IntegrationTests.Infrastructure.Extensions;
using SolutionName.IntegrationTests.PersistanceTests.Database.Fixtures;
using SolutionName.Persistence;

public class DatabaseTestEnvironmentFixture : IAsyncLifetime
{
    private readonly DatabaseConfigProvider _configProvider;
    private readonly SqlServerContainerManager _containerManager;
    public IServiceProvider ServiceProvider { get; private set; }

    public DatabaseTestEnvironmentFixture()
    {
        _configProvider = new DatabaseConfigProvider();
        var dbSettings = _configProvider.GetDatabaseSettings();
        _containerManager = new SqlServerContainerManager(dbSettings);
    }

    public async Task InitializeAsync()
    {
        await _containerManager.InitializeAsync();

        var services = new ServiceCollection()
            .AddSingleton(_configProvider.Configuration)
            .ConfigureDatabaseServices(_configProvider.GetDatabaseSettings().GetConnectionString());

        ServiceProvider = services.BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();

    }

    public async Task DisposeAsync()
    {
        await _containerManager.DisposeAsync();
    }
}
