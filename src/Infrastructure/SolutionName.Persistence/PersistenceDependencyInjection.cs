using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Application.Contracts.Persistence.UoW;
using SolutionName.Persistence.Extensions;
using SolutionName.Persistence.Identity;
using SolutionName.Persistence.UoW;

namespace SolutionName.Persistence
{
    public static class PersistenceDependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddAppIdentity();
            services.AddDbContextWithInterceptors(configuration);
            services.ScanAndRegisterRepositories();

            return services;
        }
    }
}


