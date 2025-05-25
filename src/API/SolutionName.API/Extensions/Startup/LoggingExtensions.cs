using Serilog;

namespace SolutionName.API.Extensions.Startup
{
    public static class LoggingExtensions
    {
        public static void UseLogging(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));
            var logConfiguration = new LoggerConfiguration();

            if (builder.Environment.IsDevelopment())
            {
                logConfiguration.WriteTo.Console();
            }

            Log.Logger = logConfiguration.CreateLogger();
        }
    }
}


