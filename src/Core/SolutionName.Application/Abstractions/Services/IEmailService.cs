namespace SolutionName.Application.Contracts
{
    public interface IEmailService : IService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtmlMessage = true);
    }
}


