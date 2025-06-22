namespace SolutionName.Application.Features.Auth.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string UserId, string Code, string? ChangedEmail = null) : ICommand;
} 