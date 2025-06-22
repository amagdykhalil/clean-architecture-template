namespace SolutionName.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(string Email, string ResetCode, string NewPassword) : ICommand;
} 