namespace SolutionName.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(int UserId, string OldPassword, string NewPassword) : ICommand;
}
