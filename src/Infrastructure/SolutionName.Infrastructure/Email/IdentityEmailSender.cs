// Services/EmailSender.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SolutionName.Application.Contracts;

public class IdentityEmailSender : IEmailSender<User>
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public IdentityEmailSender(IConfiguration configuration, IEmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        string subject = "Please Confirm Your Email Address";
        string htmlMessage = $@"
            <html>
            <body>
                <p>Dear {user.UserName},</p>
                <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you did not register, please ignore this email.</p>
                <br/>
                <p>Best regards,<br/>SolutionName</p>
            </body>
            </html>";

        await _emailService.SendEmailAsync(email, subject, htmlMessage);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        string subject = "Your Password Reset Code";
        string htmlMessage = $@"
            <html>
            <body>
                <p>Dear {user.UserName},</p>
                <p>You have requested to reset your password. Please use the following code to reset it:</p>
                <h2>{resetCode}</h2>
                <p>If you did not request a password reset, please ignore this email.</p>
                <br/>
                <p>Best regards,<br/>SolutionName</p>
            </body>
            </html>";

        await _emailService.SendEmailAsync(email, subject, htmlMessage);
    }


    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        string subject = "Reset Your Password";
        string htmlMessage = $@"
            <html>
            <body>
                <p>Dear {user.UserName},</p>
                <p>You have requested to reset your password. Please click the link below to reset it:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <br/>
                <p>Best regards,<br/>SolutionName</p>
            </body>
            </html>";

        await _emailService.SendEmailAsync(email, subject, htmlMessage);
    }


}

