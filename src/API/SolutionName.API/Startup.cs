using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using SolutionName.API.Extensions.Startup;
using SolutionName.API.Middleware;
using SolutionName.Application;
using SolutionName.Application.Common.Validator;
using SolutionName.Infrastructure;
using SolutionName.Persistence;

namespace SolutionName.API
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureBuilder(WebApplicationBuilder builder)
        {
            LoggingExtensions.UseLogging(builder);
            AzureKeyVaultExtensions.UseAzureKeyVault(builder);
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddProblemDetails();
            services.AddAuthorization();


            services.AddPersistence(_configuration)
                    .AddInfrastructure(_configuration)
                    .AddApplication(_configuration);

            services.AddSingleton<IFluentValidationAutoValidationResultFactory, ValidationResultFactory>();
            services.AddOpenApi();

            CorsExtensions.AddCors(services, _configuration);
            APIVersioningExtentions.AddAPIVersioning(services);
            RateLimiterExtension.AddRateLimiter(services);
        }
        public void Configure(WebApplication app)
        {

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("SolutionName API Reference")
                           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            app.UseForwardedHeaders();

            app.UseExceptionHandler();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(CorsExtensions.AllowsOrigins);

            app.MapControllers();
        }
    }
}


