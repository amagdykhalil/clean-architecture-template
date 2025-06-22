using SolutionName.Application.Abstractions.Services;

namespace SolutionName.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplate _emailTemplate;
        private readonly IEmailSender _emailSender;

        public EmailService(IEmailTemplate emailTemplate, IEmailSender emailSender)
        {
            _emailTemplate = emailTemplate;
            _emailSender = emailSender;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var emailBody = await _emailTemplate.CompileAsync(message.TemplateName, message.Placeholders);
            var compiledMessage = new CompiledEmailMessage(
                message.Subject,
                emailBody,
                message.From,
                message.To,
                message.Cc,
                message.Bcc,
                message.Attachments);

            await _emailSender.SendAsync(compiledMessage);
        }
    }
}



