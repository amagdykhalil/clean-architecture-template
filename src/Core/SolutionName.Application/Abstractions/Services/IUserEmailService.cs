using SolutionName.Domain.Entities;

namespace SolutionName.Application.Abstractions.Services
{
    /// <summary>
    /// Service interface for sending user-related emails (confirmation, password reset, etc.)
    /// </summary>
    public interface IUserEmailService
    {
        Task SendConfirmationLinkAsync(User user, string email, string confirmationLink);
        Task SendPasswordResetLinkAsync(User user, string email, string resetLink);
    }
} 