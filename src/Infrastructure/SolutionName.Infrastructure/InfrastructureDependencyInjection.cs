using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Abstractions.Infrastructure;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Application.Contracts;
using SolutionName.Infrastructure.Authentication;
using SolutionName.Infrastructure.Email;
using SolutionName.Persistence.Identity;

namespace SolutionName.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailSender<User>, IdentityEmailSender>();

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            JWTExtensions.AddJWT(services, configuration);
            return services;
        }
    }
}


