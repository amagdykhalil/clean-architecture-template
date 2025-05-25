namespace SolutionName.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string PasswordHash) : ICommand<AuthDTO>;
}
