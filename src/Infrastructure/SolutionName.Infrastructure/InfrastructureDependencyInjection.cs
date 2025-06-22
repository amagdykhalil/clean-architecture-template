using SolutionName.Application.Abstractions.Services;
using SolutionName.Infrastructure.Email;
using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Abstractions.Infrastructure;
using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Abstractions.UserContext;
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

            // Email services
            services.AddTransient<IEmailTemplate, EmailTemplate>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserEmailService, UserEmailService>();

            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            JWTExtensions.AddJWT(services, configuration);
            return services;
        }
    }
}


