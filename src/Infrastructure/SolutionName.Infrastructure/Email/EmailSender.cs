using SolutionName.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace SolutionName.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IOptions<SmtpSettings> smtpSettings,
            ILogger<EmailSender> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(CompiledEmailMessage message)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = message.Subject,
                    Body = message.Body.Html,
                    IsBodyHtml = true
                };

                // Add recipients
                foreach (var to in message.To)
                {
                    mailMessage.To.Add(new MailAddress(to.Address, to.DisplayName));
                }

                if (message.Cc != null)
                {
                    foreach (var cc in message.Cc)
                    {
                        mailMessage.CC.Add(new MailAddress(cc.Address, cc.DisplayName));
                    }
                }

                if (message.Bcc != null)
                {
                    foreach (var bcc in message.Bcc)
                    {
                        mailMessage.Bcc.Add(new MailAddress(bcc.Address, bcc.DisplayName));
                    }
                }

                // Add attachments
                if (message.Attachments != null)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        var stream = attachment.StreamFactory();
                        mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.MimeType));
                    }
                }

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email successfully sent to {Recipients}", 
                    string.Join(", ", message.To.Select(t => t.Address)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Recipients} with subject {Subject}", 
                    string.Join(", ", message.To.Select(t => t.Address)), message.Subject);
                throw;
            }
        }
    }
} 