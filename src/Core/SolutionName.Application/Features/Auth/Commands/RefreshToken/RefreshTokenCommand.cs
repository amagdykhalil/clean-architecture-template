using SolutionName.Application.Features.Auth.Models;

namespace SolutionName.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(string Token) : ICommand<AuthDTO>;
}

