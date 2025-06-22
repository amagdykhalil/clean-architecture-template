namespace SolutionName.Application.Features.Auth.Commands.ResendConfirmationEmail
{
    public record ResendConfirmationEmailCommand(string Email) : ICommand;
} 