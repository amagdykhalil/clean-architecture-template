using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Application.Abstractions.Infrastructure;
using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Infrastructure.Common.Services;
using SolutionName.Infrastructure.Email;
using SolutionName.Infrastructure.Email.Models;

namespace SolutionName.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            // Email services
            services.Configure<EmailTemplateSettings>(configuration.GetSection("EmailTemplateSettings"));
            services.Configure<EmailWorkerSettings>(configuration.GetSection("EmailWorkerSettings"));

            services.AddTransient<IEmailTemplate, EmailTemplate>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUserEmailService, UserEmailService>();
            services.AddTransient<IUserActionLinkBuilder, UserActionLinkBuilder>();

            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<IEmailQueue, InMemoryEmailQueue>();
            services.AddHostedService<EmailWorker>();

            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            return services;
        }
    }
}


