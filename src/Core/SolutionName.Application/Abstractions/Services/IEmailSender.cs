namespace SolutionName.Application.Abstractions.Services
{
    /// <summary>
    /// Sends compiled email messages using SMTP.
    /// </summary>
    public interface IEmailSender
    {
        Task SendAsync(CompiledEmailMessage message);
    }

    public record EmailAddress(string Address, string DisplayName)
    {
        public static implicit operator EmailAddress(string address) => new(address, string.Empty);
    }

    public record EmailAttachment(Func<Stream> StreamFactory, string FileName, string MimeType = "application/octet-stream");

    public record CompiledEmailMessage(
        string Subject, 
        EmailBody Body, 
        EmailAddress From,
        IEnumerable<EmailAddress> To,
        IEnumerable<EmailAddress>? Cc = null,
        IEnumerable<EmailAddress>? Bcc = null,
        IEnumerable<EmailAttachment>? Attachments = null);
} 