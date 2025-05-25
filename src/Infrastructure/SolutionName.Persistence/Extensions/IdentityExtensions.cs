using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SolutionName.Persistence.Identity;

namespace SolutionName.Persistence.Extensions
{
    public class IdentityExtensions
    {
        public static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        }
    }
}


