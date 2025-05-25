namespace SolutionName.API.Extensions.Startup
{
    public static class CorsExtensions
    {
        public static string AllowsOrigins => "AllowSpicificOrigin";
        public static void AddCors(IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(AllowsOrigins, policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowCredentials()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("Location");
                });
            });
        }
    }
}


