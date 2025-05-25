using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Application.Contracts.Persistence;
using SolutionName.Application.Contracts.Persistence.UoW;
using SolutionName.Persistence.Data.Interceptors;
using SolutionName.Persistence.Extensions;
using SolutionName.Persistence.Identity;
using SolutionName.Persistence.Interceptors;
using SolutionName.Persistence.Repositories;
using SolutionName.Persistence.UoW;

namespace SolutionName.Persistence
{
    public static class PersistenceDependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<UpdateAuditableInterceptor>();
            IdentityExtensions.AddIdentity(services);
            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    // Resolve interceptors from the container
                    .AddInterceptors(
                        provider.GetRequiredService<SoftDeleteInterceptor>(),
                        provider.GetRequiredService<UpdateAuditableInterceptor>()
                    )
                    .LogTo(Console.WriteLine, LogLevel.Information);
            });


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }
    }
}


