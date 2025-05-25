using Microsoft.AspNetCore.Identity;
using SolutionName.Application.Abstractions.UserContext;

namespace SolutionName.Persistence.Identity
{
    /// <summary>
    /// Service for managing user identity operations using ASP.NET Core Identity.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public IdentityService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<User?> GetUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var result = await _userManager.CheckPasswordAsync(user, password);
            return result ? user : null;
        }
        public async Task<IList<string>> GetRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Enumerable.Empty<string>().ToList();
            return await _userManager.GetRolesAsync(user);
        }


        public async Task AddToRoleAsync(int userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return;

            if (!await _roleManager.RoleExistsAsync(role))
                return;
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> ValidatePasswordAsync(string password)
        {
            // We pass null as the user because we only care about the rules,
            // not whether it matches an existing user�s password.
            return await _userManager.PasswordValidators[0]
                          .ValidateAsync(_userManager, null!, password);
        }

    }
}


