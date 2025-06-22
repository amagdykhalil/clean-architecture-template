using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionName.Application.Abstractions.Services;
using SolutionName.Infrastructure.Common.Services;
using SolutionName.Persistence;
using SolutionName.Persistence.Extensions;
using SolutionName.Persistence.Identity;
using SolutionName.Persistence.Repositories;
using SolutionName.Persistence.UoW;

namespace SolutionName.IntegrationTests.Infrastructure.Extensions
{
    public static class DatabaseServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDatabaseServices(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("No connection string was found.");
            }

            return services
                .ConfigureLogging()
                .ConfigureIdentity()
                .ConfigureDbContext(connectionString)
                .ConfigureRepositories();
        }

        private static IServiceCollection ConfigureLogging(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Warning);
            });

            return services;
        }

        private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            IdentityExtensions.AddAppIdentity(services);
            return services;
        }

        private static IServiceCollection ConfigureDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        private static IServiceCollection ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
