namespace SolutionName.Application.Abstractions.Services
{
    /// <summary>
    /// Provides functionality to compile email templates into formatted HTML strings.
    /// </summary>
    public interface IEmailTemplate
    {
        Task<EmailBody> CompileAsync(string templateName, IEnumerable<Placeholder> placeholders);
    }

    public record EmailBody(string Html, string PlainText);
    public record Placeholder(string Name, string Value);
} 