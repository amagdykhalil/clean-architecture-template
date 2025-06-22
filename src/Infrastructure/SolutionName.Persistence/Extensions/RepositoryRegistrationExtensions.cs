using SolutionName.Application.Contracts.Persistence.Base;
using Microsoft.Extensions.DependencyInjection;

namespace SolutionName.Persistence.Extensions
{
    public static class RepositoryRegistrationExtensions
    {
        public static IServiceCollection ScanAndRegisterRepositories(this IServiceCollection services)
        {
            services.Scan(scan => scan
                 .FromAssembliesOf(typeof(PersistenceDependencyInjection))
                 .AddClasses(classes => classes.AssignableTo<IRepository>())
                 .As((type) =>
                     type.GetInterfaces()
                         .Where(i => typeof(IRepository).IsAssignableFrom(i) && i != typeof(IRepository))
                 )
                 .AsImplementedInterfaces()
                 .WithScopedLifetime()
             );

            return services;
        }
    }

}
