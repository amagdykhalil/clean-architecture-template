namespace SolutionName.Application.Features.Auth.Commands.RevokeToken
{
    public record RevokeTokenCommand(string? Token) : ICommand;
}
