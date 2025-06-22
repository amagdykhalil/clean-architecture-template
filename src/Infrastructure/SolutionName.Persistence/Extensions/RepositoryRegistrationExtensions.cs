using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Contracts.Persistence.Base;

namespace SolutionName.Persistence.Extensions
{
    public static class RepositoryRegistrationExtensions
    {
        public static IServiceCollection ScanAndRegisterRepositories(this IServiceCollection services)
        {
            services.Scan(scan => scan
                 .FromAssembliesOf(typeof(PersistenceDependencyInjection))
                 .AddClasses(classes => classes.AssignableTo<IRepository>())
                 .AsImplementedInterfaces()
                 .WithScopedLifetime()
             );

            return services;
        }
    }

}
