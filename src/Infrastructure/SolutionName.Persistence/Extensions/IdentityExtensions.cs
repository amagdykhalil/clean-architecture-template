using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Domain.Entities;

namespace SolutionName.Persistence.Extensions
{
    public static class IdentityExtensions
    {
        public static void AddAppIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        }
    }
}


