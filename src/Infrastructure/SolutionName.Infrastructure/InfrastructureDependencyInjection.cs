using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Contracts;
using SolutionName.Application.Contracts.Persistence.Base;
using SolutionName.Infrastructure.Authentication;
using SolutionName.Infrastructure.Email;
using SolutionName.Persistence.Identity;

namespace SolutionName.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailSender<User>, IdentityEmailSender>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(InfrastructureDependencyInjection))
                .AddClasses(classes => classes.AssignableTo<IService>())
                .As((type) =>
                    type.GetInterfaces()
                        .Where(i => typeof(IRepository).IsAssignableFrom(i) && i != typeof(IRepository))
                )
                .WithScopedLifetime()
            );


            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            JWTExtensions.AddJWT(services, configuration);
            return services;
        }
    }
}


