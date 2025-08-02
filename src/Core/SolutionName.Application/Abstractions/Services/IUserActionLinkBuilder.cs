namespace SolutionName.Application.Abstractions.Services
{
    public interface IUserActionLinkBuilder
    {
        /// <summary>
        /// Generates an email‑confirmation token for the given user,
        /// encodes it, and builds the full confirmation URL.
        /// </summary>
        Task<string> BuildEmailConfirmationLinkAsync(
            User user,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a password reset token for the given user,
        /// encodes it, and builds the full reset password URL.
        /// </summary>
        Task<string> BuildPasswordResetLinkAsync(
            User user,
            string email,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a change-email token for the given user and new email,
        /// encodes it, and builds the full confirmation URL.
        /// </summary>
        Task<string> BuildChangeEmailConfirmationLinkAsync(
            User user,
            string newEmail,
            CancellationToken cancellationToken = default);

        // Add more methods as needed for other user actions
    }
}