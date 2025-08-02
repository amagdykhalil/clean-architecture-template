using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Application.Common.Models;
using SolutionName.Domain.Entities;
using SolutionName.Domain.Enums;
using System.Data;

namespace SolutionName.Persistence.Identity
{
    /// <summary>
    /// Service for managing user identity operations using ASP.NET Core Identity.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly AppDbContext _context;

        public IdentityService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<User?> GetUserAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var result = await _userManager.CheckPasswordAsync(user, password);
            return result ? user : null;
        }

        public async Task<IList<string?>> GetRolesAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new List<string?>();

            var userRoleNames = await _userManager.GetRolesAsync(user); // These are the role names (Name)

            return await _roleManager.Roles
                .Where(r => userRoleNames.Contains(r.Name))
                .Select(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return;
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return;
            await _userManager.AddToRoleAsync(user, role.Name);
        }

        public async Task<IdentityResult> ValidatePasswordAsync(string password, CancellationToken cancellationToken = default)
        {
            // We pass null as the user because we only care about the rules,
            // not whether it matches an existing user's password.
            return await _userManager.PasswordValidators[0]
                          .ValidateAsync(_userManager, null!, password);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _userManager.CreateAsync(user);
        }

        // User management methods
        public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User?> GetUserByIdIncludePersonAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user; // Return the user or null if not found
        }

        public async Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
        // Email confirmation methods
        public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsEmailConfirmedAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string code, CancellationToken cancellationToken = default)
        {
            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string code, CancellationToken cancellationToken = default)
        {
            return await _userManager.ChangeEmailAsync(user, newEmail, code);
        }

        public async Task<IdentityResult> SetUserNameAsync(User user, string userName, CancellationToken cancellationToken = default)
        {
            return await _userManager.SetUserNameAsync(user, userName);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail, CancellationToken cancellationToken = default)
        {
            return await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        }

        // Password reset methods
        public async Task<string> GeneratePasswordResetTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string code, string newPassword, CancellationToken cancellationToken = default)
        {

            return await _userManager.ResetPasswordAsync(user, code, newPassword);
        }

        public async Task AddUserRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return;
            var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
            await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task RemoveFromRolesAsync(IEnumerable<int> roleIds, CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync(cancellationToken);
            if (roles.Any())
            {
                _context.Set<IdentityRole<int>>().RemoveRange(roles);
            }
        }

        public async Task<User?> FindByEmailIncludePersonAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userManager.Users.Include(e => e.Person).Where(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<int?> IsExitsByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await _userManager.Users
               .Where(u => u.Email == email)
               .Select(u => (int?)u.Id)
               .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<List<IdentityRole<int>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _roleManager.Roles.ToListAsync(cancellationToken);
        }

        public async Task<List<IdentityRole<int>>> GetAllUserRolesAsync(int userId, CancellationToken cancellationToken = default)
        {
            // Get all role IDs for the user from the UserRoles join table
            var roleIds = await _context.Set<IdentityUserRole<int>>()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            // Get the Role entities for those IDs
            var roles = await _roleManager.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            return roles;
        }

        public async Task<IdentityRole<int>?> GetRoleAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }
    }
}



