using System.Security.Claims;
using System.Threading.RateLimiting;

namespace SolutionName.API.Extensions.Startup
{
    public static class RateLimiterExtension
    {
        public static void AddGlobalRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1)
                        }));
            });

        }
    }
}

