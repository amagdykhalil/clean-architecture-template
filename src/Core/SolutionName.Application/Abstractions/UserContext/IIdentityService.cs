using Microsoft.AspNetCore.Identity;
using SolutionName.Domain.Enums;
using System.Data;

namespace SolutionName.Application.Abstractions.UserContext
{
    /// <summary>
    /// Service interface for handling user identity and authentication operations.
    /// </summary>
    public interface IIdentityService
    {
        Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<User?> GetUserAsync(string email, string password, CancellationToken cancellationToken = default);

        Task<IdentityResult> ValidatePasswordAsync(string password, CancellationToken cancellationToken = default);
        Task<IdentityResult> CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<int?> IsExitsByEmail(string email, CancellationToken cancellationToken = default);

        // User management methods
        Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<User?> GetUserByIdIncludePersonAsync(int userId, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteUserAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

        // Email confirmation methods
        Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> FindByEmailIncludePersonAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsEmailConfirmedAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ConfirmEmailAsync(User user, string code, CancellationToken cancellationToken = default);
        Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string code, CancellationToken cancellationToken = default);
        Task<IdentityResult> SetUserNameAsync(User user, string userName, CancellationToken cancellationToken = default);
        Task<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail, CancellationToken cancellationToken = default);

        // Password reset methods
        Task<string> GeneratePasswordResetTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ResetPasswordAsync(User user, string code, string newPassword, CancellationToken cancellationToken = default);

        // Role management
        Task<IList<string?>> GetRolesAsync(int userId, CancellationToken cancellationToken = default);
        Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
        Task AddUserRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken = default);
        Task<List<IdentityRole<int>>> GetAllUserRolesAsync(int userId, CancellationToken cancellationToken = default);
        Task RemoveFromRolesAsync(IEnumerable<int> roleIds, CancellationToken cancellationToken = default);
        Task<List<IdentityRole<int>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
        Task<IdentityRole<int>?> GetRoleAsync(int id, CancellationToken cancellationToken = default);
    }
}

