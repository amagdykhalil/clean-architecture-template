using Microsoft.AspNetCore.Identity;

namespace SolutionName.Application.Abstractions.UserContext
{
    /// <summary>
    /// Service interface for handling user identity and authentication operations.
    /// </summary>
    public interface IIdentityService
    {
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<User?> GetUserAsync(string email, string password);
        Task<IList<string>> GetRolesAsync(int userId);
        Task AddToRoleAsync(int userId, string role);
        Task<IdentityResult> ValidatePasswordAsync(string password);
        Task<IdentityResult> CreateUserAsync(User user, string password);

        // Email confirmation methods
        Task<User?> FindByIdAsync(string userId);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> IsEmailConfirmedAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string code);
        Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string code);
        Task<IdentityResult> SetUserNameAsync(User user, string userName);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);

        // Password reset methods
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string code, string newPassword);
    }
}

