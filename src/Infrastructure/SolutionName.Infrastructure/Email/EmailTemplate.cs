using SolutionName.Application.Abstractions.Services;
using SolutionName.Shared.Keys;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SolutionName.Infrastructure.Email
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly Assembly _assembly;

        public EmailTemplate()
        {
            _assembly = typeof(LocalizationKeys).Assembly;
        }

        public async Task<EmailBody> CompileAsync(string templateName, IEnumerable<Placeholder> placeholders)
        {
            var template = await LoadTemplateAsync(templateName);
            var html = ReplacePlaceholders(template, placeholders);
            var plainText = ConvertHtmlToPlainText(html);

            return new EmailBody(html, plainText);
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var resourceName = $"SolutionName.Shared.Templates.{templateName}.html";

            using var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException($"Template '{templateName}' not found as embedded resource.");
            }

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private string ReplacePlaceholders(string template, IEnumerable<Placeholder> placeholders)
        {
            var result = template;
            foreach (var placeholder in placeholders)
            {
                result = result.Replace($"{{{{{placeholder.Name}}}}}", placeholder.Value);
            }
            return result;
        }

        private string ConvertHtmlToPlainText(string html)
        {
            // Simple HTML to plain text conversion
            var plainText = html
                .Replace("<br/>", "\n")
                .Replace("<br>", "\n")
                .Replace("</p>", "\n\n")
                .Replace("</div>", "\n");

            // Remove HTML tags
            plainText = Regex.Replace(plainText, "<[^>]*>", "");

            // Decode HTML entities
            plainText = System.Web.HttpUtility.HtmlDecode(plainText);

            // Clean up whitespace
            plainText = Regex.Replace(plainText, @"\n\s*\n", "\n\n");
            plainText = plainText.Trim();

            return plainText;
        }
    }
}