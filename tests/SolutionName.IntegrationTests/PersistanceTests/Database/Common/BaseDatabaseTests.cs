using SolutionName.Persistence;

namespace SolutionName.IntegrationTests.PersistanceTests.Database.Common
{
    [Collection(TestCollections.DatabaseTests)]
    public abstract class BaseDatabaseTests : IAsyncLifetime
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly Func<Task> ResetDatabase;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly AppDbContext DbContext;
        protected BaseDatabaseTests(DatabaseTestEnvironmentFixture fixture)
        {
            UnitOfWork = fixture.ServiceProvider.GetRequiredService<IUnitOfWork>();
            DbContext = fixture.DbContext;
            ResetDatabase = fixture.ResetDatabase;
            ServiceProvider = fixture.ServiceProvider;
        }
        public virtual async Task InitializeAsync()
        {
            await ResetDatabase();
        }
        public virtual async Task DisposeAsync() => await ResetDatabase();

    }
}
