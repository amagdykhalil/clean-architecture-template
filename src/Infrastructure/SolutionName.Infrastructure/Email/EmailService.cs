using SolutionName.Application.Abstractions.Services;

namespace SolutionName.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplate _emailTemplate;
        private readonly IEmailQueue _emailQueue;

        public EmailService(IEmailTemplate emailTemplate, IEmailQueue emailQueue)
        {
            _emailTemplate = emailTemplate;
            _emailQueue = emailQueue;
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

            await _emailQueue.EnqueueEmailAsync(compiledMessage);
        }
    }
}



