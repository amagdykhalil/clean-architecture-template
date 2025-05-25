using SolutionName.Application.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace SolutionName.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<SmtpSettings> smtpSettings,
            ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtmlMessage = true)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true
                })
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtmlMessage,
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }
                _logger.LogInformation("Email successfully sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending email to {Email} with subject {Subject}", toEmail, subject);
                throw;
            }
        }
    }
}


