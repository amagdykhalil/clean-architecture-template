using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Abstractions.Infrastructure;
using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Application.Contracts;
using SolutionName.Infrastructure.Authentication;
using SolutionName.Infrastructure.Common.Services;
using SolutionName.Infrastructure.Email;
using SolutionName.Infrastructure.Localization;

namespace SolutionName.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddLocalizationSetup();

            services.AddScoped<IEmailSender<User>, IdentityEmailSender>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            JWTExtensions.AddJWT(services, configuration);
            return services;
        }
    }
}


