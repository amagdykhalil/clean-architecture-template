using SolutionName.IntegrationTests.Infrastructure.Extensions;
using SolutionName.IntegrationTests.PersistanceTests.Database.Fixtures;
using SolutionName.Persistence;
using Microsoft.EntityFrameworkCore;
using Respawn;
using System.Data.Common;

public class DatabaseTestEnvironmentFixture : IAsyncLifetime
{
    private readonly DatabaseConfigProvider _configProvider;
    private readonly SqlServerContainerManager _containerManager;

    private IServiceScope _scope;
    private Respawner _respawner;
    private DbConnection _connection;

    public AppDbContext DbContext { get; private set; }
    public IServiceProvider ServiceProvider => _scope.ServiceProvider;

    public DatabaseTestEnvironmentFixture()
    {
        _configProvider = new DatabaseConfigProvider();
        var dbSettings = _configProvider.GetDatabaseSettings();
        _containerManager = new SqlServerContainerManager(dbSettings);
    }

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        await _containerManager.InitializeAsync();

        var services = new ServiceCollection()
            .AddSingleton(_configProvider.Configuration)
            .ConfigureDatabaseServices(
                _containerManager.dbSettings.GetConnectionString(),
                _configProvider.Configuration);

        var rootProvider = services.BuildServiceProvider();


        _scope = rootProvider.CreateScope();
        var sp = _scope.ServiceProvider;

        // Resolve AppDbContext and apply migrations
        DbContext = sp.GetRequiredService<AppDbContext>();
        await DbContext.Database.MigrateAsync();

        var respawnerOptions = new RespawnerOptions
        {
            SchemasToInclude = new[]
            {
                "dbo"
            },
            DbAdapter = DbAdapter.SqlServer
        };

        _connection = DbContext.Database.GetDbConnection();
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, respawnerOptions);
        await ResetDatabase();
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_containerManager.dbSettings.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await _containerManager.DisposeAsync();
    }
}
